using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 플레이어가 소유한 모든 아이템, 재화 관리.
/// </summary>
public class InventoryManager
{
    private int _nextInstanceId = 0;
    public int Gold { get; private set; } = 0; // 플레이어가 소유한 골드
    public Dictionary<int, ItemInstanceData> ItemDict { get; private set; } = new Dictionary<int, ItemInstanceData>(); // key: instanceId, value: ItemData
    public List<ItemInstanceData> ItemList => ItemDict.Values.ToList();

    #region Item
    /// <summary>
    /// itemDataId에 해당하는 아이템을 quantity 수만큼 인벤토리에 추가.
    /// </summary>
    public void AddItem(int itemDataId, int quantity = 1)
    {
        if(!Managers.DataMng.ItemDataDict.TryGetValue(itemDataId, out ItemData itemData))
        {
            Debug.LogError($"[InventoryManager] Could not add item with itemDataId: {itemDataId}. No such item data exists.");
            return;
        }

        ItemInstanceData savedItemData = ItemList.Find(e => e.ItemDataId == itemDataId);

        if (savedItemData != null && savedItemData is ConsumableItemInstanceData consumableItemData)
        {
            consumableItemData.Quantity += quantity; // 이미 존재하는 소비 아이템의 경우 수량만 증가시킴
        }
        else
        {
            switch(itemData.ItemType)
            {
                case ItemType.Consumable:
                    ItemDict.Add(_nextInstanceId, new ConsumableItemInstanceData(itemDataId, _nextInstanceId++, quantity));
                    break;
                case ItemType.Weapon:
                case ItemType.Armor:
                    for(int i = 0; i < quantity; i++)
                    {
                        ItemDict.Add(_nextInstanceId, new EquipmentInstanceData(itemDataId, _nextInstanceId++));
                    }
                    break;
                default:
                    Debug.LogError($"[InventoryManager] Could not add item with itemDataId: {itemDataId}. Unknown item type: {itemData.ItemType}.");
                    return;
            }
        }
    }

    /// <summary>
    /// instanceId에 해당하는 아이템 인스턴스의 개수를 quantity만큼 제거. 개수가 0이될 시 인벤토리에서 완전히 제거.
    /// </summary>
    /// <returns>
    /// 정상적으로 제거 시 true, 문제가 발생하여 제거하지 못할 시 false 반환.
    /// </returns>
    public bool RemoveItem(int instanceId, int quantity = 1)
    {
        if (!ItemDict.TryGetValue(instanceId, out ItemInstanceData itemData))
        {
            Debug.LogError($"[InventoryManager] Could not remove item with instanceId: {instanceId}. No such item exists.");
            return false;
        }

        if (itemData is ConsumableItemInstanceData consumableItemData)
        {
            if(consumableItemData.Quantity < quantity)
            {
                Debug.LogError($"[InventoryManager] Not enough quantity to remove. Requested: {quantity}, Available: {consumableItemData.Quantity}");
                return false;
            }

            consumableItemData.Quantity -= quantity;
            if (consumableItemData.Quantity <= 0)
            {
                ItemDict.Remove(instanceId);
            }
        }
        else
        {
            ItemDict.Remove(instanceId);
        }
        return true;
    }

    /// <summary>
    /// item을 quantity만큼 remove함과 동시에, 판매 가격에 맞게 골드 획득
    /// </summary>
    public bool SellItem(ItemInstanceData item, int quantity = 1)
    {
        if (!RemoveItem(item.InstanceId, quantity))
        {
            return false;
        }

        Gold += item.ItemData.SellPrice * quantity;

        return true;
    }

    public ItemInstanceData GetItemByDataId(int dataId)
    {
        return ItemList.FirstOrDefault(e => e.ItemDataId == dataId);
    }

    /// <summary>
    /// dataId를 데이터로 하는 장비 중, 영웅에게 장착되지 않은 첫 장비의 Instance를 반환.
    /// </summary>
    public EquipmentInstanceData GetUnequippedEquipment(int dataId)
    {
        return ItemList
            .Where(e => e is EquipmentInstanceData equipment && equipment.EquippedHeroId == -1 && e.ItemDataId == dataId)
            .Select(e => (EquipmentInstanceData)e)
            .FirstOrDefault();
    }

    /// <summary>
    /// 특정 아이템 타입에 해당하는 모든 아이템을 반환.
    /// </summary>
    public List<ItemInstanceData> GetAllItemsOfType(ItemType itemType)
    {
        return ItemList
            .Where(e => e.ItemType == itemType)
            .ToList();
    }

    /// <summary>
    /// 특정 장비 타입에 해당하는 모든 장비 아이템을 반환.
    /// </summary>
    public List<ItemInstanceData> GetAllEquipmentOfType(EquipmentType equipmentType)
    {
        return ItemList
            .Where(e => e is EquipmentInstanceData equipment && equipment.EquipmentType == equipmentType)
            .ToList();
    }

    /// <summary>
    /// 주어진 InstanceId가 유효한 장비인지 확인하고, 해당 장비의 타입이 일치하는지 검증.
    /// </summary>
    public bool IsValidEquipment(int instanceId, EquipmentType type)
    {
        if(!ItemDict.TryGetValue(instanceId, out ItemInstanceData itemData))
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
    #endregion
    #region Currency
    public void AddGold(int amount)
    {
        if(amount < 0)
        {
            Debug.LogError("[InventoryManager] Cannot add negative gold amount.");
            return;
        }
        Gold += amount;
    }

    public void RemoveGold(int amount)
    {
        if(amount < 0)
        {
            Debug.LogError("[InventoryManager] Cannot remove negative gold amount.");
            return;
        }
        if(Gold < amount)
        {
            Debug.LogError("[InventoryManager] Not enough gold to remove.");
            return;
        }
        Gold -= amount;
    }
    #endregion
}
