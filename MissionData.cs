using NLua;

namespace DCSDynamicTemplateHelper;

internal sealed class MissionData {
    public MissionData(
        LuaTable missionTable,
        LuaTable warehousesTable,
        List<DCSTemplateGroupInfo> groupsInMission,
        long maxGroupId,
        long maxUnitId) {
        MissionTable = missionTable;
        WarehousesTable = warehousesTable;
        GroupsInMission = groupsInMission;
        MaxGroupId = maxGroupId;
        MaxUnitId = maxUnitId;
    }

    public LuaTable MissionTable { get; }

    public LuaTable WarehousesTable { get; }

    public List<DCSTemplateGroupInfo> GroupsInMission { get; }

    public long MaxGroupId { get; }

    public long MaxUnitId { get; }
}

