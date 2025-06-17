using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class QuestReward
{
    public int ItemDataId;
    public int Quantity;
}

[Serializable]
public class QuestData
{
    public int DataId;
    public int Chapter;
    public int Stage;

    public string Name;
    public string Objective;

    public string AreaName; // 퀘스트 수락 시 이동되는 Area. AreaName enum의 값과 같아야함.
    public QuestReward[] Rewards;
    public int[] UnlockQuestDataId; // 해당 퀘스트 완료 시 열리는 퀘스트의 DataId

    public bool IsUnlocked; // 퀘스트 개방 여부
    public bool IsComplete; // 퀘스트 완료 여부. 처음은 무조건 false.
}

[Serializable]
public class QuestDataLoader : ILoader<int, QuestData>
{
    public List<QuestData> quests = new List<QuestData>();

    public Dictionary<int, QuestData> MakeDict()
    {
        var dic = new Dictionary<int, QuestData>();
        foreach (QuestData quest in quests)
        {
            if (!Enum.TryParse(quest.AreaName, out AreaName areaName))
            {
                Debug.LogError($"Quest {quest.DataId} - AreaName is invalid!");
                continue;
            }
            dic.Add(quest.DataId, quest);
        }
        return dic;
    }
}
