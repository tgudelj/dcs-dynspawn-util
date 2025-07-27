using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCSDynamicTemplateHelper;
public class AppSettings {
    public AppSettings() {
        Flyable = new List<DCSNameTypeItem>();
    }

    public List<DCSNameTypeItem> Flyable { get; set; }
}

public class DCSNameTypeItem {
    public DCSNameTypeItem() {

    }

    public DCSNameTypeItem(string displayName, string dCSType) {
        DisplayName = displayName;
        DCSType = dCSType;
    }

    public string DisplayName {  get; set; }
    public string DCSType { get; set; }
}
