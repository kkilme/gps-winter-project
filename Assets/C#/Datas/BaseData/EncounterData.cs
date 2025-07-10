using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EncounterData
{
    public int DataId;
    public string Name;
    public string ClassName; // Encounter 클래스 이름
    public string Description;
    public string ImagePath;
    public bool IsLeavable; // 플레이어가 시도하지 않고 떠날 수 있는지 여부
}

[Serializable]
public class EncounterDataLoader : ILoader<int, EncounterData>
{
    public List<EncounterData> encounterDatas = new List<EncounterData>();
    public Dictionary<int, EncounterData> MakeDict()
    {
        var dic = new Dictionary<int, EncounterData>();
        foreach (var data in encounterDatas)
        {
            dic.Add(data.DataId, data);
        }
        return dic;
    }
}