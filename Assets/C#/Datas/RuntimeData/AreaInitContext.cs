using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Area 초기화에 필요한 정보를 보유하는 클래스
/// </summary>
public class AreaInitContext
{
    public AreaName AreaName { get; private set; } // Area 이름
    public Quest Quest { get; private set; } // Area에 해당하는 퀘스트 정보
    public List<ItemData> Items { get; private set; } // Area에 가져갈 아이템들

    public AreaInitContext(Quest quest, List<ItemData> items)
    {
        if (!Enum.TryParse(quest.QuestData.AreaName, out AreaName areaName))
        {
            Debug.LogError($"[AreaInitContext] Invalid AreaName: {quest.QuestData.AreaName}");
            return;
        }
        AreaName = areaName;
        Quest = quest;
        Items = items;
    }
}

/// <summary>
/// 테스트용 AreaInitContext 클래스
/// </summary>
public class TestAreaInitContext: AreaInitContext
{
    public TestAreaInitContext(): 
        base(new Quest(Managers.DataMng.QuestDataDict.Values.ToList()[0]), 
        new List<ItemData>()
        { // 테스트용 아이템들
            Managers.DataMng.ItemDataDict[GlobalValues.ITEM_HEALPOTION_ID],
            Managers.DataMng.ItemDataDict[GlobalValues.ITEM_HEALPOTION_ID],
            Managers.DataMng.ItemDataDict[GlobalValues.ITEM_HEALPOTION_ID],
            Managers.DataMng.ItemDataDict[GlobalValues.ITEM_HEALPOTION_ID],
            Managers.DataMng.ItemDataDict[GlobalValues.ITEM_HEALPOTION_ID],
            Managers.DataMng.ItemDataDict[GlobalValues.ITEM_HEALPOTION_ID],
        })
    { }
}
