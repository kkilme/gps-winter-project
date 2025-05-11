using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 소유한 모든 아이템 관리.
/// </summary>
public class Inventory
{
    public List<InventorySlot> ItemSlots { get; private set; }

    public Inventory()
    {
        ItemSlots = new List<InventorySlot>();
    }

    public void AddItem(Item item, int quantity)
    {
        InventorySlot slot = ItemSlots.Find(s => s.Item == item);
        if (slot != null)
        {
            
        }
        else
        {
            ItemSlots.Add(new InventorySlot(item, quantity));
        }
    }

}
