using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 플레이어가 소유한 모든 아이템 관리.
/// </summary>
public class InventoryManager
{
    private int _nextInstanceId = 0;
    public Dictionary<int, ItemInstanceData> SavedItemDatas { get; private set; } = new Dictionary<int, ItemInstanceData>();
    public List<ItemInstanceData> SavedItemDataList => SavedItemDatas.Values.ToList();

    public void AddItem(int dataId, int quantity = 1)
    {
        if(!Managers.DataMng.ItemDataDict.TryGetValue(dataId, out ItemData itemData))
        {
            Debug.LogError($"[InventoryManager] Could not add item with dataId: {dataId}. No such item data exists.");
            return;
        }

        ItemInstanceData savedItemData = SavedItemDataList.Find(e => e.ItemDataId == dataId);

        if (savedItemData != null && savedItemData is ConsumableItemInstanceData consumableItemData)
        {
            consumableItemData.Quantity += quantity; // 이미 존재하는 소비 아이템의 경우 수량만 증가시킴
        }
        else
        {
            switch(itemData.ItemType)
            {
                case ItemType.Consumable:
                    SavedItemDatas.Add(_nextInstanceId, new ConsumableItemInstanceData(dataId, _nextInstanceId++, quantity));
                    break;
                case ItemType.Weapon:
                case ItemType.Armor:
                    for(int i = 0; i < quantity; i++)
                    {
                        SavedItemDatas.Add(_nextInstanceId, new EquipmentInstanceData(dataId, _nextInstanceId++));
                    }
                    break;
                default:
                    Debug.LogError($"[InventoryManager] Could not add item with dataId: {dataId}. Unknown item type: {itemData.ItemType}.");
                    return;
            }
        }
    }

    public ItemInstanceData GetItemByDataId(int dataId)
    {
        return SavedItemDataList.FirstOrDefault(e => e.ItemDataId == dataId);
    }

    public EquipmentInstanceData GetUnequippedEquipment(int dataId)
    {
        return SavedItemDataList
            .Where(e => e is EquipmentInstanceData equipment && equipment.EquippedHeroId == -1 && e.ItemDataId == dataId)
            .Select(e => (EquipmentInstanceData)e)
            .FirstOrDefault();
    }

    public List<ItemInstanceData> GetAllItemsOfType(ItemType itemType)
    {
        return SavedItemDataList
            .Where(e => e.ItemType == itemType)
            .ToList();
    }

    public List<ItemInstanceData> GetAllEquipmentOfType(EquipmentType equipmentType)
    {
        return SavedItemDataList
            .Where(e => e is EquipmentInstanceData equipment && equipment.EquipmentType == equipmentType)
            .ToList();
    }

    public bool HaveItem(int dataId, int quantity = 1)
    {
       return SavedItemDataList.Any(e => e.ItemDataId == dataId && (e is ConsumableItemInstanceData consumable ? consumable.Quantity >= quantity : true));
    }

    /// <summary>
    /// 주어진 InstanceId가 유효한 장비인지 확인하고, 해당 장비의 타입이 일치하는지 검증.
    /// </summary>
    public bool IsValidEquipment(int instanceId, EquipmentType type)
    {
        if(!SavedItemDatas.TryGetValue(instanceId, out ItemInstanceData itemData))
        {
            Debug.LogError($"[InventoryManager] Invalid instanceId: {instanceId}");
            return false;
        }

        if(itemData is not EquipmentInstanceData equipmentData)
        {
            Debug.LogError($"[InventoryManager] InstanceId {instanceId} is not an EquipmentInstanceData.");
            return false;
        }

        if(equipmentData.EquipmentType != type)
        {
            Debug.LogError($"[InventoryManager] InstanceId {instanceId} is not of type {type}. Actual type: {equipmentData.EquipmentType}");
            return false;
        }

        return true;
    }
}
