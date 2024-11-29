using System;
using System.Collections.Generic;
using UnityEngine;

public class Bag
{
    public Hero Owner { get; set; }
    public List<BaseItem> Items { get; protected set; }
    public int Gold { get; protected set; }
    public event Action<Bag> ContentChange;

    public void SetInfo()
    {
        Items = new List<BaseItem>(6);
        for (int i = 0; i < 6; i++)
            Items.Add(null);
        
        Gold = 0;
    }
    
    public BaseItem StoreItem(int itemDataId, int addNum = 1)
    {
        int currentItemIdx = IsInBag(itemDataId);
        if (currentItemIdx != -1)
        {
            Items[currentItemIdx].Count += addNum;
            ContentChange?.Invoke(this);
            return Items[currentItemIdx];
        }
        
        string className = Managers.DataMng.ItemDataDict[itemDataId].Name;
        BaseItem item = Activator.CreateInstance(Type.GetType(className)) as BaseItem;
        if (item == null)
            return null;
        
        for (int idx = 0; idx < Items.Count; idx++)
        {
            if (Items[idx] == null)
            {
                item.SetInfo(itemDataId, Owner, this, idx, addNum);
                Items[idx] = item;
                break;
            }
            Debug.Log("Failed to StoreItem: Bag is Full");
        }
        
        ContentChange?.Invoke(this);
        return item;
    }
    
    // 비전투 중 아이템 사용
    public void UseItem(int idx, ulong targetId)
    {
        
    }

    public int IsInBag(int itemDataId)
    {
        for (int idx = 0; idx < Items.Count; idx++)
        {
            if (Items[idx] == null)
                continue;
            
            if (Items[idx].DataId == itemDataId)
                return idx;
        }

        return -1;
    }
}
