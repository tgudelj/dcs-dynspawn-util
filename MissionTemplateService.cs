using NLua;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Text;

namespace DCSDynamicTemplateHelper;

internal sealed class MissionTemplateService {
    private readonly Lua _lua;

    public MissionTemplateService() {
        _lua = new Lua();
        _lua.State.Encoding = Encoding.UTF8;
    }

    public MissionData LoadMissionFile(string filename) {
        List<DCSTemplateGroupInfo> groupsInMission = new();
        LuaTable? missionTable = null;
        LuaTable? warehousesTable = null;
        long maxGroupId = 1;
        long maxUnitId = 1;

        using (ZipArchive archive = ZipFile.Open(filename, ZipArchiveMode.Update)) {
            ZipArchiveEntry missionLuaFile = archive.GetEntry("mission")
                ?? throw new InvalidDataException("Mission archive is missing 'mission' entry.");
            ZipArchiveEntry warehouseLuaFile = archive.GetEntry("warehouses")
                ?? throw new InvalidDataException("Mission archive is missing 'warehouses' entry.");

            using (StreamReader sr = new(warehouseLuaFile.Open())) {
                string warehousesLua = sr.ReadToEnd();
                var warehouses = _lua.DoString(warehousesLua + Environment.NewLine + "return warehouses");
                warehousesTable = (LuaTable)warehouses[0];
            }

            using (StreamReader sr = new(missionLuaFile.Open())) {
                string missionLua = sr.ReadToEnd();
                var mission = _lua.DoString(missionLua + Environment.NewLine + "return mission");
                missionTable = (LuaTable)mission[0];
                LuaTable coalitionsTable = missionTable["coalition"] as LuaTable
                    ?? throw new InvalidDataException("Mission table is missing 'coalition'.");

                foreach (string coalitionKey in coalitionsTable.Keys.Cast<string>()) {
                    LuaTable coalitionTable = coalitionsTable[coalitionKey] as LuaTable
                        ?? throw new InvalidDataException($"Coalition '{coalitionKey}' table is invalid.");
                    LuaTable countriesTable = coalitionTable["country"] as LuaTable
                        ?? throw new InvalidDataException($"Coalition '{coalitionKey}' is missing 'country' table.");

                    for (int countryIndex = 1; countryIndex <= countriesTable.Keys.Count; countryIndex++) {
                        LuaTable countryTable = countriesTable[countryIndex] as LuaTable
                            ?? throw new InvalidDataException($"Country index '{countryIndex}' table is invalid.");

                        foreach (string vehicleCategory in countryTable.Keys.Cast<string>()) {
                            LuaTable? vehicleCategoryTable = countryTable[vehicleCategory] as LuaTable;
                            if (vehicleCategoryTable == null) {
                                continue;
                            }

                            LuaTable groupsTable = vehicleCategoryTable["group"] as LuaTable
                                ?? throw new InvalidDataException($"Vehicle category '{vehicleCategory}' missing 'group' table.");

                            for (int groupIndex = 1; groupIndex <= groupsTable.Keys.Count; groupIndex++) {
                                LuaTable group = groupsTable[groupIndex] as LuaTable
                                    ?? throw new InvalidDataException($"Group index '{groupIndex}' table is invalid.");
                                long groupId = (long)group["groupId"];
                                if (groupId > maxGroupId) {
                                    maxGroupId = groupId;
                                }

                                if (group.Keys.Cast<string>().Contains("dynSpawnTemplate")) {
                                    LuaTable unitsTable = group["units"] as LuaTable
                                        ?? throw new InvalidDataException($"Group '{groupId}' missing 'units' table.");
                                    LuaTable firstUnit = unitsTable[1] as LuaTable
                                        ?? throw new InvalidDataException($"Group '{groupId}' has invalid first unit.");
                                    DCSTemplateGroupInfo groupInfo = new() {
                                        GroupId = (long)group["groupId"],
                                        GroupName = group["name"] as string,
                                        GroupTable = group,
                                        DCSVehicleType = firstUnit["type"] as string,
                                        Coalition = coalitionKey,
                                        Country = countryIndex
                                    };
                                    groupsInMission.Add(groupInfo);

                                    for (int unitIndex = 1; unitIndex <= unitsTable.Keys.Count; unitIndex++) {
                                        LuaTable unitTable = unitsTable[unitIndex] as LuaTable
                                            ?? throw new InvalidDataException($"Group '{groupId}' unit '{unitIndex}' is invalid.");
                                        long unitId = (long)unitTable["unitId"];
                                        if (unitId > maxUnitId) {
                                            maxUnitId = unitId;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return new MissionData(
            missionTable ?? throw new InvalidDataException("Failed to parse mission table."),
            warehousesTable ?? throw new InvalidDataException("Failed to parse warehouses table."),
            groupsInMission,
            maxGroupId,
            maxUnitId);
    }

    public void ApplyTemplateAndSave(
        string missionFilePath,
        MissionData loadedMission,
        DCSTemplateGroupInfo selectedTemplateGroup,
        IReadOnlyCollection<DCSTypeInfo> selectedTypes) {
        long currentGroupId = loadedMission.MaxGroupId + 1;
        long currentUnitId = loadedMission.MaxUnitId + 1;
        Dictionary<string, long> typeTemplateGroupMap = new();

        int i = 0;
        foreach (DCSTypeInfo typeItem in selectedTypes) {
            Debug.WriteLine($@"Creating dynamic template group for {typeItem.DisplayName} [{typeItem.DCSType}]");
            LuaTable clone = CloneLuaTable(selectedTemplateGroup.GroupTable);
            string groupPrefix = $"DST-{typeItem.DCSType}";
            clone["dynSpawnTemplate"] = true;
            clone["name"] = groupPrefix;
            clone["groupId"] = currentGroupId;
            clone["tasks"] = DeserializeLuaTable("{}");
            clone["frequency"] = null;
            clone["task"] = "Nothing";
            clone["password"] = GenerateRandomPassword(); //We do not really care about the password, it is there just to prevent players spawning as this group
            LuaTable unitsTable = clone["units"] as LuaTable
                ?? throw new InvalidDataException("Template group clone is missing units table.");
            LuaTable firstUnit = unitsTable[1] as LuaTable
                ?? throw new InvalidDataException("Template group clone has invalid first unit.");
            firstUnit["name"] = $"{groupPrefix}-1";
            firstUnit["unitId"] = currentUnitId;
            firstUnit["type"] = typeItem.DCSType;
            firstUnit["skill"] = "Client";
            firstUnit["Radio"] = DeserializeLuaTable("{}");
            firstUnit["AddPropAircraft"] = DeserializeLuaTable("{}");
            LuaTable payloadTable = firstUnit["payload"] as LuaTable
                ?? throw new InvalidDataException("Template group first unit is missing payload table.");
            payloadTable["pylons"] = DeserializeLuaTable("{}");
            typeTemplateGroupMap.Add(typeItem.DCSType, currentGroupId);

            double x = Convert.ToDouble(firstUnit["x"], CultureInfo.InvariantCulture);
            double y = Convert.ToDouble(firstUnit["y"], CultureInfo.InvariantCulture);
            x += i * 10;
            y += i * 10;
            firstUnit["x"] = x;
            firstUnit["y"] = y;

            LuaTable firstPoint = GetTableByPath(["route", "points", 1L], clone);
            firstPoint["x"] = x;
            firstPoint["y"] = y;

            LuaTable injectTable = GetTableByPath([
                "coalition",
                selectedTemplateGroup.Coalition,
                "country",
                selectedTemplateGroup.Country,
                typeItem.Category,
                "group"
            ], loadedMission.MissionTable, true);
            int count = injectTable.Keys.Count;
            injectTable[count + 1] = clone;

            i++;
            currentGroupId++;
            currentUnitId++;
        }

        string mission = "mission = " + SerializeLuaTableFast(loadedMission.MissionTable);

        LuaTable airportsTable = loadedMission.WarehousesTable["airports"] as LuaTable
            ?? throw new InvalidDataException("Warehouses table is missing airports.");
        foreach (KeyValuePair<object, object> item in airportsTable) {
            LuaTable airport = item.Value as LuaTable
                ?? throw new InvalidDataException("Airport table is invalid.");
            airport["allowHotStart"] = true;
            LinkDynamicTemplates(typeTemplateGroupMap, airport);
        }

        LuaTable farpsTable = loadedMission.WarehousesTable["warehouses"] as LuaTable
            ?? throw new InvalidDataException("Warehouses table is missing FARP warehouses.");
        foreach (KeyValuePair<object, object> item in farpsTable) {
            LuaTable farp = item.Value as LuaTable
                ?? throw new InvalidDataException("FARP table is invalid.");
            farp["allowHotStart"] = true;
            LinkDynamicTemplates(typeTemplateGroupMap, farp);
        }

        string warehouses = "warehouses = " + SerializeLuaTableFast(loadedMission.WarehousesTable);

        using (ZipArchive archive = ZipFile.Open(missionFilePath, ZipArchiveMode.Update)) {
            ZipArchiveEntry missionLuaFile = archive.GetEntry("mission")
                ?? throw new InvalidDataException("Mission archive is missing 'mission' entry.");
            ZipArchiveEntry warehousesLuaFile = archive.GetEntry("warehouses")
                ?? throw new InvalidDataException("Mission archive is missing 'warehouses' entry.");

            missionLuaFile.Delete();
            warehousesLuaFile.Delete();
            missionLuaFile = archive.CreateEntry("mission");
            warehousesLuaFile = archive.CreateEntry("warehouses");
            using (StreamWriter writer = new(missionLuaFile.Open())) {
                writer.Write(mission);
            }

            using (StreamWriter writer = new(warehousesLuaFile.Open())) {
                writer.Write(warehouses);
            }
        }
    }

    private void LinkDynamicTemplates(Dictionary<string, long> typeTemplateGroupMap, LuaTable warehouseNode) {
        LuaTable helicopters = GetTableByPath(["aircrafts", "helicopters"], warehouseNode, true);
        foreach (KeyValuePair<object, object> aircraft in helicopters) {
            if (aircraft.Key is string dcsType && typeTemplateGroupMap.ContainsKey(dcsType)) {
                (aircraft.Value as LuaTable)["linkDynTempl"] = typeTemplateGroupMap[dcsType];
                (aircraft.Value as LuaTable)["initialAmount"] = 1L;
            }
        }

        LuaTable planes = GetTableByPath(["aircrafts", "planes"], warehouseNode, true);
        foreach (KeyValuePair<object, object> aircraft in planes) {
            if (aircraft.Key is string dcsType && typeTemplateGroupMap.ContainsKey(dcsType)) {
                (aircraft.Value as LuaTable)["linkDynTempl"] = typeTemplateGroupMap[dcsType];
                (aircraft.Value as LuaTable)["initialAmount"] = 1L;
            }
        }
    }

    private LuaTable GetTableByPath(object[] path, LuaTable table, bool createIfNecessary = true) {
        LuaTable currentTable = table;
        for (int i = 0; i < path.Length; i++) {
            object key = path[i];
            if (currentTable == null) {
                throw new InvalidDataException("Current table is null, cannot proceed with path lookup.");
            }

            if (!currentTable.Keys.Cast<object>().ToList().Contains(key)) {
                if (createIfNecessary) {
                    currentTable[key] = DeserializeLuaTable("{}");
                } else {
                    throw new KeyNotFoundException($"Key '{key}' not found in the table.");
                }
            }

            currentTable = currentTable[key] as LuaTable;
        }

        return currentTable
            ?? throw new InvalidDataException("Table path resolved to null.");
    }

    private string SerializeLuaTableFast(object obj, string ident = "") {
        StringBuilder sb = new();
        if (obj is string s) {
            sb.AppendFormat("\"{0}\"", s.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\"", "\\\"").Replace("\n", "\\\n"));
        } else if (obj is long l) {
            sb.Append(l);
        } else if (obj is double d) {
            sb.Append(d);
        } else if (obj is bool b) {
            sb.Append(b.ToString().ToLower());
        } else if (obj is LuaTable table) {
            sb.Append("{\n");
            foreach (KeyValuePair<object, object> item in table) {
                sb.Append(ident);
                if (item.Key is string) {
                    sb.AppendFormat("[\"{0}\"] = ", item.Key);
                } else if (item.Key is long || item.Key is double) {
                    sb.AppendFormat("[{0}] = ", item.Key);
                }
                sb.Append(SerializeLuaTableFast(item.Value, ident + "   "));
                sb.Append(",\n");
            }
            sb.Append(ident);
            sb.Append("}");
        } else {
            throw new InvalidDataException($"Can't serialize {obj.GetType()}");
        }

        return sb.ToString();
    }

    private string SerializeLuaTable(LuaTable table) {
        const string luaSerializer = @"
function ___serializeTable(tbl)
	local out = """"		
		local function serialize (o, ident)
		  if (ident == nil) then
			  ident = "" ""
		  end
		  if type(o) == ""number"" then
			out = out..o
		  elseif type(o) == ""string"" then
			out = out..string.format(""%q"", o)
		  elseif type(o) == ""boolean"" then
			out = out..string.format(""%s"", o)
		  elseif type(o) == ""table"" then
			out = out..""{\n""
			for k,v in pairs(o) do
			   out = out..ident..""   ""
			   if(type(k) == ""string"") then
				out = out..string.format(""[%q] = "", k)
			   elseif(type(k) == ""number"") then
				out=out..string.format(""[%d] = "", k)
			   end          
			  serialize(v, ident..""   "")
			  out = out.."",\n""
			end
			out = out..ident..""}""
		  else
			error(""cannot serialize a "" .. type(o).. ""\nSerialized so far: ""..out)
		  end
		end
		serialize(tbl)
		io.write(out)
		return out
    end
        ";
        _lua.DoString($"{luaSerializer}\n");
        var serializeFunc = _lua["___serializeTable"] as LuaFunction
            ?? throw new InvalidDataException("Lua serializer function could not be created.");
        return (string)serializeFunc.Call(table).First();
    }

    private LuaTable CloneLuaTable(LuaTable table) {
        string serialized = SerializeLuaTable(table);
        _lua.DoString($@"function ___returnTable() 
                                return {serialized}
                            end");
        var func = _lua["___returnTable"] as LuaFunction
            ?? throw new InvalidDataException("Lua clone function could not be created.");
        return func.Call().First() as LuaTable
            ?? throw new InvalidDataException("Failed to clone Lua table.");
    }

    private LuaTable DeserializeLuaTable(string tableLua) {
        _lua.DoString($@"function ___returnTable() 
                                return {tableLua}
                            end");
        var func = _lua["___returnTable"] as LuaFunction
            ?? throw new InvalidDataException("Lua deserialize function could not be created.");
        return func.Call().First() as LuaTable
            ?? throw new InvalidDataException("Failed to deserialize Lua table.");
    }

    /// <summary>
    /// Generates a random password consisting of alphanumeric characters.
    /// </summary>
    /// <remarks>The password is generated using a combination of uppercase letters, lowercase letters, and
    /// digits. The method does not guarantee uniqueness of the generated password.</remarks>
    /// <returns>A string representing the generated random password, which is 10 characters long.</returns>
    private string GenerateRandomPassword() {
        var allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        int passwordLength = 10;
        var random = new Random();
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < passwordLength; i++) {
            sb.Append(allowedChars[random.Next(passwordLength -1)]);
        }
        return sb.ToString();
    }
}


