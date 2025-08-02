using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class AreaData
{
    public string Name; // AreaName enum의 값과 같아야 함
    public string BattleFieldName; // 전투 필드의 프리팹 이름
    public int CollapseTimer; // Collapse 주기
    public int CollapseAmount; // Collapse 시 파괴되는 행 개수
    public int MaxRestCount; // 최대 휴식 가능 횟수
    public List<int> MonsterSquadIds = new(); // 이 Area에서 등장 가능한 몬스터 스쿼드의 DataId
    public int BossSquadId; // 이 Area의 Boss 스쿼드의 DataId
    public List<int> EncounterIds = new(); // 이 Area에서 등장 가능한 Encounter의 DataId
}

[Serializable]
public class AreaDataSet : ILoader<AreaName, AreaData>
{
    public List<AreaData> areadatas = new();

    public Dictionary<AreaName, AreaData> MakeDict()
    {
        var dic = new Dictionary<AreaName, AreaData>();
        foreach (AreaData areadata in areadatas)
        {
            if (Enum.TryParse(areadata.Name, out AreaName areaName))
            {
                dic.Add(areaName, areadata);
            }
            else
            {
                Debug.LogError($"{areadata.Name} - AreaName is invalid!");
            }
        }
        return dic;
    }
}
