using Microsoft.Extensions.Configuration;
using NLua;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Compression;
using System.Linq;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DCSDynamicTemplateHelper;
public partial class frmMain : Form {
    Lua lua = new Lua();    
    AppSettings settings;
    List<DCSTemplateGroupInfo> GroupsInMission = new List<DCSTemplateGroupInfo>();
    LuaTable MissionTable = null;
    internal BindingList<DCSTemplateGroupInfo> _groups = new BindingList<DCSTemplateGroupInfo>();
    internal BindingList<DCSTypeInfo> _typesList = new BindingList<DCSTypeInfo>();
    DCSTemplateGroupInfo SelectedTemplatGroup = null;
    long MaxGroupId = 1; //Maximum group id found in mission
    long MaxUnitId = 1; //Maximum unit id found in mission

    public frmMain() {
        InitializeComponent();
    }

    #region ------------- Event handlers -------------

    private void frmMain_Load(object sender, EventArgs e) {
        lua.State.Encoding = Encoding.UTF8;
        settings = Program.Configuration.GetSection("Settings").Get<AppSettings>();
        foreach (DCSTypeInfo item in settings.Flyable) {
            _typesList.Add(item);
        }
        lbApplyTo.DataSource = _typesList;
        lbApplyTo.DisplayMember = "DisplayName";
        lbApplyTo.SelectedIndex = -1;

        lbMizGroups.DataSource = _groups;
        lbMizGroups.DisplayMember = "DisplayName";
    }

    private void btnCancel_Click(object sender, EventArgs e) {
        Application.Exit();
    }

    private void btnSelectAll_Click(object sender, EventArgs e) {
        for (int i = 0; i < lbApplyTo.Items.Count; i++) {
            lbApplyTo.SetSelected(i, true);
        }
    }

    private void btnClear_Click(object sender, EventArgs e) {
        lbApplyTo.SelectedIndex = -1;
    }

    private void btnClearGroupsFilter_Click(object sender, EventArgs e) {
        txtGroupsFilter.Text = "";
        _groups.Clear();
        foreach (DCSTemplateGroupInfo g in GroupsInMission) {
            _groups.Add(g);
        }
    }

    private void btnClearApplyToFilter_Click(object sender, EventArgs e) {
        txtApplyToFilter.Text = "";
        _typesList.Clear();
        foreach (DCSTypeInfo item in settings.Flyable) { 
            _typesList.Add(item);
        }
    }

    private void txtGroupsFilter_KeyUp(object sender, KeyEventArgs e) {
        string searchString = txtGroupsFilter.Text;
        _groups.Clear();
        if (searchString.Length < 2) {
            foreach (DCSTemplateGroupInfo group in GroupsInMission) {
                _groups.Add(group);
            }
            return; 
        }
        foreach (DCSTemplateGroupInfo g in GroupsInMission) {
            if (g.GroupName.Contains(searchString) || g.DCSVehicleType.Contains(searchString)) {
                _groups.Add(g);
            }
        }
    }

    private void txtApplyToFilter_KeyUp(object sender, KeyEventArgs e) {
        string searchString = txtApplyToFilter.Text;
        _typesList.Clear();
        if (searchString.Length < 2) {
            foreach (DCSTypeInfo item in settings.Flyable) {
                _typesList.Add(item);
            }
            return; 
        }
        foreach (DCSTypeInfo t in settings.Flyable) {
            if (t.DisplayName.Contains(searchString)) {
                _typesList.Add(t);
            }
        }
    }

    private void lbMizGroups_SelectedIndexChanged(object sender, EventArgs e) {
        if (lbMizGroups.SelectedItems.Count == 0) {
            SelectedTemplatGroup = null;
            return;
        }
        SelectedTemplatGroup = lbMizGroups.SelectedItem as DCSTemplateGroupInfo;
        lblSelectedGroupInfo.Text = $"Name: {SelectedTemplatGroup.GroupName}\nType: {SelectedTemplatGroup.DCSVehicleType}\nGroupId {SelectedTemplatGroup.GroupId}";
    }

    private void lbApplyTo_SelectedIndexChanged(object sender, EventArgs e) {
        List<DCSTypeInfo> selectedItems = new List<DCSTypeInfo>();
        foreach (int i in lbApplyTo.SelectedIndices) {
            selectedItems.Add(lbApplyTo.Items[i] as DCSTypeInfo);
        }
    }

    private void miOpenMission_Click(object sender, EventArgs e) {
        DialogResult result = dlgOpenFile.ShowDialog();
        if (result != DialogResult.OK) {
            return;
        }
        //Create a backup
        string fileName = dlgOpenFile.FileName;
        FileInfo original = new FileInfo(fileName);
        string bakcup = original.FullName + DateTime.Now.ToString("_yyyy-MM-dd-HH-mm-ss") + ".bak";
        File.Copy(original.FullName, bakcup);
        //Clear list box
        _groups.Clear();
        GroupsInMission.Clear();
        //Load groups and bind list box
        LoadMission(fileName);
    }

    #endregion

    private void LoadMission(string filename) {
        using (ZipArchive archive = ZipFile.Open(filename, ZipArchiveMode.Update)) {
            ZipArchiveEntry missionLuaFile = archive.GetEntry("mission");
            using (StreamReader sr = new StreamReader(missionLuaFile.Open())) {
                string missionLua = sr.ReadToEnd();
                var mission = lua.DoString(missionLua + Environment.NewLine + "return mission");
                MissionTable = (LuaTable)mission[0];
                LuaTable coalitionsTable = MissionTable["coalition"] as LuaTable;
                //iterate coalitions
                foreach (string coalitionKey in coalitionsTable.Keys.Cast<string>()) {
                    LuaTable coalitionTable = coalitionsTable[coalitionKey] as LuaTable;
                    LuaTable countriesTable = coalitionTable["country"] as LuaTable;
                    //iterate countries, index based
                    for (int countryIndex = 1; countryIndex <= countriesTable.Keys.Count; countryIndex++) {
                        LuaTable countryTable = countriesTable[countryIndex] as LuaTable;
                        //iterate vehicle types
                        foreach (string vehicleCategory in countryTable.Keys.Cast<string>()) {
                            LuaTable vehicleCategoryTable = countryTable[vehicleCategory] as LuaTable;
                            if (vehicleCategoryTable != null) {
                                LuaTable groupsTable = vehicleCategoryTable["group"] as LuaTable;
                                //iterate groups, index based
                                for (int groupIndex = 1; groupIndex <= groupsTable.Keys.Count; groupIndex++) {
                                    //Inspect group
                                    LuaTable group = groupsTable[groupIndex] as LuaTable;
                                    long groupId = (long)group["groupId"];
                                    //We need the highest groupId so we know where to start when inserting new groups
                                    Debug.WriteLine($"groupId: {groupId} Max: {MaxGroupId}");
                                    if (groupId > MaxGroupId) { 
                                        MaxGroupId = groupId;
                                    }
                                    if (group.Keys.Cast<string>().Contains("dynSpawnTemplate")) {
                                        LuaTable unitsTable = group["units"] as LuaTable;
                                        LuaTable firstUnit = unitsTable[1] as LuaTable;
                                        DCSTemplateGroupInfo groupInfo = new DCSTemplateGroupInfo();
                                        groupInfo.GroupId = (long)group["groupId"];
                                        groupInfo.GroupName = group["name"] as string;
                                        groupInfo.GroupTable = group;
                                        groupInfo.DCSVehicleType = firstUnit["type"] as string;
                                        groupInfo.Coalition = coalitionKey;
                                        groupInfo.Country = countryIndex;
                                        GroupsInMission.Add(groupInfo);
                                        //iterate units because we must find the highest unitId so we know where to start when inserting new ones
                                        //index based
                                        for (int unitIndex = 1; unitIndex <= unitsTable.Keys.Count; unitIndex++) {
                                            LuaTable unitTable = unitsTable[unitIndex] as LuaTable;
                                            long unitId = (long)unitTable["unitId"];
                                            Debug.WriteLine($"unitId: {unitId} Max: {MaxUnitId}");
                                            if (unitId > MaxUnitId) { 
                                                MaxUnitId = unitId;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        _groups.Clear();
        foreach (DCSTemplateGroupInfo group in GroupsInMission) { 
            _groups.Add(group);
        }
        lbMizGroups.SelectedIndex = -1;
    }

    private LuaTable GetTableByPath(object[] path, LuaTable table) {
        LuaTable currentTable = table;
        for (int i = 0; i < path.Length; i++) {
            object key = path[i];
            if (currentTable != null) {
                if (!currentTable.Keys.Cast<object>().ToList().Contains(key)) {
                    throw new KeyNotFoundException($"Key '{key}' not found in the table.");
                } else {

                }
                currentTable = currentTable[key] as LuaTable;
            } else {
                throw new Exception("Current table is null, cannot proceed with path lookup.");
            }
        }
        return currentTable;
    }

    private void btnApply_Click(object sender, EventArgs e) {
        //Add required groups to mission, set ["dynSpawnTemplate"] = true to added groups and remember their groupIds
        if (SelectedTemplatGroup == null) {
            return;
        }
        if (lbApplyTo.SelectedItems.Count == 0) {
            return;
        }
        long currentGroupId = MaxGroupId + 1;
        long currentUnitId = MaxUnitId + 1;
        //build a dictionary of dcs type to template group id, we will need it when modifying warehouse
        Dictionary<string, long> TypeTemplateGroupMap = new Dictionary<string, long>();
        //Add groups to mission
        foreach (DCSTypeInfo typeItem in lbApplyTo.SelectedItems) {
            Debug.WriteLine($@"Creating dynamic template group for {typeItem.DisplayName} [{typeItem.DCSType}]");
            LuaTable clone = CloneLuaTable(SelectedTemplatGroup.GroupTable);
            string groupPrefix = $"DST-{typeItem.DCSType}";
            clone["dynSpawnTemplate"] = true;
            clone["name"] = groupPrefix;
            clone["groupId"] = currentGroupId;
            clone["tasks"] = DeserializeLuaTable("{}");
            clone["task"] = "Nothing";
            LuaTable unitsTable = clone["units"] as LuaTable;
            LuaTable firstUnit = unitsTable[1] as LuaTable;
            firstUnit["name"] = $"{groupPrefix}-1";
            firstUnit["unitId"] = currentUnitId;
            firstUnit["type"] = typeItem.DCSType;
            //? Payload
            LuaTable payloadTable = firstUnit["payload"] as LuaTable;
            payloadTable["pylons"] = DeserializeLuaTable("{}");
            //? Fuel
            //Now assemble the table backwards



            TypeTemplateGroupMap.Add(typeItem.DCSType, currentGroupId);
            //we need to insert this new group into 
            //coalition/red\blue|neutrals/country/[?]/plane|helicopter/group/
            //LuaTable injectTable = GetTableByPath(["coalition", "neutrals", "country", 1L, typeItem.Category], MissionTable);
            //LuaTable injectTable = GetTableByPath(["coalition", "neutrals", "country", 1L], MissionTable);
            //if (!injectTable.Keys.Cast<string>().ToList().Contains(typeItem.Category)) {
            //    //no entry for our category, we need to add it
            //    injectTable[typeItem.Category] = DeserializeLuaTable("{}");                
            //}

            LuaTable injectTable = ((LuaTable)((LuaTable)((LuaTable)((LuaTable)MissionTable["coalition"])["neutrals"])["country"])[1]);
            if (!injectTable.Keys.Cast<string>().ToList().Contains(typeItem.Category)) {
                injectTable[typeItem.Category] = DeserializeLuaTable("{}");
            }

            int count = ((LuaTable)injectTable[typeItem.Category]).Keys.Count;
            ((LuaTable)injectTable[typeItem.Category])[count + 1] = clone;

            currentGroupId++;
            currentUnitId++;
        }
        string s = SerializeLuaTable(MissionTable);
        Debug.WriteLine(s);
        //Add ["linkDynTempl"] = groupId to each warehouse
    }

    private string SerializeLuaTable(LuaTable table) {
        string luaSerializer = @"
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
        lua.DoString($"{luaSerializer}\n");
        var serializeFunc = lua["___serializeTable"] as LuaFunction;
        var res = (string)serializeFunc.Call(table).First();
        return res;
    }

    private LuaTable CloneLuaTable(LuaTable table) { 
        string serialized = SerializeLuaTable(table);
        lua.DoString($@"function ___returnTable() 
                                return {serialized}
                            end");
        var func = lua["___returnTable"] as LuaFunction;
        LuaTable newTable = func.Call().First() as LuaTable;
        return newTable;
    }

    private LuaTable DeserializeLuaTable(string tableLua) {
        lua.DoString($@"function ___returnTable() 
                                return {tableLua}
                            end");
        var func = lua["___returnTable"] as LuaFunction;
        LuaTable newTable = func.Call().First() as LuaTable;
        return newTable;
    }

    private LuaTable AddToIndexTable(LuaTable table, LuaTable tableToInsert) {
        string luaString = @"
        function ___addToTable(table1, table2)
            table.insert(table, tableToInsert)
            return table1
        end
        ";
        lua.DoString(luaString);
        var func = lua["___addToTable"] as LuaFunction;
        LuaTable newTable = func.Call(table, tableToInsert).First() as LuaTable;
        return newTable;
    }
}
