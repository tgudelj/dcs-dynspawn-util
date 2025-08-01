using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCSDynamicTemplateHelper;
public class AppSettings {
    public AppSettings() {
        Flyable = new List<DCSTypeInfo>();
    }

    public List<DCSTypeInfo> Flyable { get; set; }
}

public class DCSTypeInfo {
    public DCSTypeInfo() {

    }

    public DCSTypeInfo(string displayName, string dCSType, string category) {
        DisplayName = displayName;
        DCSType = dCSType;
        Category = category;
    }

    public string DisplayName {  get; set; }
    public string DCSType { get; set; }
    public string Category { get; set; }
}
