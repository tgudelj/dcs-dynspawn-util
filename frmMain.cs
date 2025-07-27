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
    public frmMain() {
        InitializeComponent();
    }

    private void miOpenMission_Click(object sender, EventArgs e) {
        DialogResult result = dlgOpenFile.ShowDialog();
        if (result != DialogResult.OK) {
            return;
        }
        Lua lua = new Lua();
        lua.State.Encoding = Encoding.UTF8;
        string fileName = dlgOpenFile.FileName;
        FileInfo original = new FileInfo(fileName);
        string bakcup = original.FullName + DateTime.Now.ToString("_yyyy-MM-dd-HH-mm-ss") + ".bak";
        File.Copy(original.FullName, bakcup);
        using (ZipArchive archive = ZipFile.Open(fileName, ZipArchiveMode.Update)) {
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
                                        Debug.WriteLine($"group: {firstUnit["type"]} {group["name"]}");
                                        Debug.WriteLine($"maxIndex: {maxIndex}");
                                        if ((bool)group["dynSpawnTemplate"] == true) {
                                            Console.WriteLine("Found a template group");
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
        settings = Program.Configuration.GetSection("settings").Get<AppSettings>();
    }
}
