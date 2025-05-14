using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 플레이어가 소유한 모든 아이템 관리.
/// </summary>
public class InventoryManager
{
    public List<InventoryEntry> ItemEntries { get; private set; }

    public InventoryManager()
    {
        ItemEntries = new List<InventoryEntry>();
    }

    public void AddItem(int dataId, int quantity)
    {
        InventoryEntry entry = ItemEntries.Find(e => e.DataId == dataId);
        if (entry != null)
        {
            entry.AddQuantity(quantity);
        }
        else
        {
            ItemEntries.Add(new InventoryEntry(dataId, quantity));
        }
    }

    public List<InventoryEntry> GetAllItemsOfType(ItemType itemType)
    {
        return ItemEntries
            .Where(entry => entry.ItemData.ItemType == itemType)
            .ToList();
    }

    public bool HaveItem(int dataId, int quantity = 1)
    {
        return ItemEntries.Any(entry => entry.DataId == dataId && entry.Quantity >= quantity);
    }

}
