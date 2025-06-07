using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class StoreData
{
    public ItemType ItemType; // 아이템 타입 (소모품, 장비 등)
    public List<StoreEntryData> StoreEntries = new List<StoreEntryData>(); // 상점 아이템 목록
}

public class StoreEntryData
{
    public int ItemDataId; // 아이템의 DataId
    public int Price; // 상점에서의 가격
    public bool HasStockLimit = false; // 재고 제한이 있는지 여부 (무제한 재고는 false로 설정)
    public int Stock; // (재고가 무제한이 아닐 시)재고량
}

[Serializable]
public class StoreDataLoader : ILoader<ItemType, StoreData>
{
    public List<StoreData> StoreDatas = new List<StoreData>();
    public Dictionary<ItemType, StoreData> MakeDict()
    {
        var dic = new Dictionary<ItemType, StoreData>();
        foreach (var store in StoreDatas)
        {
            dic.Add(store.ItemType, store);
        }
        return dic;
    }
}