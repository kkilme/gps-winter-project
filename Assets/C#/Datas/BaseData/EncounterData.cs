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
    public StatName UsingStat;
    public int CoinCount;
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