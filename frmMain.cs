using NLua;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace DCSDynamicTemplateHelper;
public partial class frmMain : Form {

    AppSettings settings;
    List<DCSTemplateGroupInfo> mizGroups = new List<DCSTemplateGroupInfo>();
    LuaTable mizTable = null;
    public frmMain() {
        InitializeComponent();
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
        lbMizGroups.DataBindings.Clear();
        lbMizGroups.DataSource = null;
        lbMizGroups.Items.Clear();
        mizGroups.Clear();
        //Load groups and bind list box
        LoadGroupsFromMission(fileName);
        lbMizGroups.DataSource = mizGroups;
        lbMizGroups.DisplayMember = "DisplayName";
    }

    private void LoadGroupsFromMission(string filename) {
        Lua lua = new Lua();
        lua.State.Encoding = Encoding.UTF8;
        using (ZipArchive archive = ZipFile.Open(filename, ZipArchiveMode.Update)) {
            ZipArchiveEntry missionLuaFile = archive.GetEntry("mission");
            using (StreamReader sr = new StreamReader(missionLuaFile.Open())) {
                string missionLua = sr.ReadToEnd();
                var mission = lua.DoString(missionLua + Environment.NewLine + "return mission");
                LuaTable missionTable = (LuaTable)mission[0];
                LuaTable coalitionsTable = GetTableWithPath("coalition", missionTable);
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
                                    //FindForm the max index in groups table, we might need it later to insert new template groups
                                    int maxIndex = 1;
                                    foreach (var index in groupsTable.Keys.Cast<long>()) {
                                        if (index > maxIndex) {
                                            maxIndex = (int)index;
                                        }
                                    }
                                    if (group.Keys.Cast<string>().Contains("dynSpawnTemplate")) {
                                        LuaTable unitsTable = group["units"] as LuaTable;
                                        LuaTable firstUnit = unitsTable[1] as LuaTable;
                                        DCSTemplateGroupInfo groupInfo = new DCSTemplateGroupInfo();
                                        groupInfo.GroupId = (long)group["groupId"];
                                        groupInfo.GroupName = group["name"] as string;
                                        groupInfo.GroupTable = group;
                                        groupInfo.MAXIndexInCategory = maxIndex;
                                        groupInfo.DCSVehicleType = firstUnit["type"] as string;
                                        groupInfo.Coalition = coalitionKey;
                                        groupInfo.Country = countryIndex;
                                        mizGroups.Add(groupInfo);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private LuaTable GetTableWithPath(string path, LuaTable table) {
        string[] arrPath = path.Split("/");

        LuaTable currentTable = table;
        string currentPath = "";
        for (int i = 0; i < arrPath.Length; i++) {
            string key = arrPath[i];
            currentPath += "/" + key;
            if (currentTable != null) {
                if (!currentTable.Keys.Cast<string>().ToList().Contains(key)) {
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

    private void frmMain_Load(object sender, EventArgs e) {
        settings = Program.Configuration.GetSection("Settings").Get<AppSettings>();
        lbApplyTo.DataSource = settings.Flyable;
        lbApplyTo.DisplayMember = "DisplayName";
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

    private void btnApply_Click(object sender, EventArgs e) {
        //Add required groups to mission, set ["dynSpawnTemplate"] = true to added groups and remember their groupIds
        string ser = Serialize(mizGroups[0].GroupTable);

        //Add ["linkDynTempl"] = groupId to each warehouse
    }

    private string Serialize(LuaTable table) {
        string luaSerializer = @"
    function serialize (o, ident)
      if (ident == nil) then
          ident = "" ""
      end
      if (type(o) == mil or o == nil) then
        io.write(""nil"")
      elseif type(o) == ""number"" then
        io.write(o)
      elseif type(o) == ""string"" then
        io.write(string.format(""%q"", o))
      elseif type(o) == ""boolean"" then
        io.write(string.format(""%s"", o))
      elseif type(o) == ""table"" then
        io.write(""{\n"")
        for k,v in pairs(o) do
           io.write(ident..""   "")
		   if(type(k) == ""string"") then
			io.write(string.format(""[%q] = "", k))
		   elseif(type(k) == ""number"") then
			io.write(""["", k, ""\""] = "")
		   end          
          serialize(v, ident..""   "")
          io.write("",\n"")
        end
        io.write(ident..""}"")
      else
        error(""cannot serialize a "" .. type(o))
      end
    end
        ";
        Lua lua = new Lua();
        lua.State.Encoding = Encoding.UTF8;
        lua.DoString($"{luaSerializer}\n");
        var serializeFunc = lua["serialize"] as LuaFunction;
        var res = (string)serializeFunc.Call(table).First();
        return res;
    }

    private void lbMizGroups_SelectedIndexChanged(object sender, EventArgs e) {
        if (lbMizGroups.SelectedItems.Count == 0) {
            return;
        }
        DCSTemplateGroupInfo selectedGroup = lbMizGroups.SelectedItem as DCSTemplateGroupInfo;
        lblSelectedGroupInfo.Text = $"Name: {selectedGroup.GroupName}\nType: {selectedGroup.DCSVehicleType}\nGroupId {selectedGroup.GroupId}";
    }

    private void lbApplyTo_SelectedIndexChanged(object sender, EventArgs e) {
        List< DCSNameTypeItem> selectedItems = new List< DCSNameTypeItem>();
        foreach (int i in lbApplyTo.SelectedIndices) {
            selectedItems.Add(lbApplyTo.Items[i] as DCSNameTypeItem);
        }
    }
}
