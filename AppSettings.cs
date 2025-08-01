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

    public DCSTypeInfo(string displayName, string dCSType, string category, long frequency) {
        DisplayName = displayName;
        DCSType = dCSType;
        Category = category;
        Frequency = frequency;
    }

    public string DisplayName {  get; set; }
    public string DCSType { get; set; }
    public long Frequency {  get; set; }
    public string Category { get; set; }
}
