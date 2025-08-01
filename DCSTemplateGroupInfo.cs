using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCSDynamicTemplateHelper;
internal class DCSTemplateGroupInfo {
    public string GroupName { get; set; }

    public long GroupId { get; set; }

    public LuaTable GroupTable { get; set; }

    public int MAXIndexInCategory { get; set; }

    public string DCSVehicleType { get; set; }

    public string Coalition { get; set; }

    public long Country { get; set; }

    public string DisplayName {
        get{
        return $"[{Coalition}] {this.DCSVehicleType} {this.GroupName} [{this.GroupId}]";
        }
    }
}
