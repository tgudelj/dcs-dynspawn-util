using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCSDynamicTemplateHelper;
internal class DCSTemplateGroupInfo {
    public string name { get; set; }

    public string groupId { get; set; }

    public LuaTable route { get; set;}

    public int MAXIndexInCategory { get; set; }
}
