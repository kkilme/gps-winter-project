using System;
using System.Collections;
using UnityEngine;


public class InventoryEntry
{
    public int DataId { get; private set; }
    public ItemData ItemData => Managers.DataMng.ItemDataDict[DataId];
    public int Quantity { get; private set; }

     /*  public Item ItemInstance { get; private set; } */// 강화나 내구도 등 '상태'를 저장해야 할 시 다른 방식으로 저장할 필요가 있음. 현재로선 계획 x.

    public InventoryEntry(int dataId, int quantity)
    {
        DataId = dataId;
        Quantity = quantity;
    }

    public void AddQuantity(int quantity)
    {
        Quantity += quantity;
    }
}