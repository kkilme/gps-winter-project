using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class ItemFactory
{
    private static Dictionary<int, ItemData> _itemDatas => Managers.DataMng.ItemDataDict;

    public static Item CreateItemById(int dataId)
    {
        if (!_itemDatas.TryGetValue(dataId, out var itemData))
        {
            Debug.LogError($"[ItemFactory]Could not create item form id: {dataId}. No such id is in dictionary.");
            return null;
        }

        switch (itemData.ItemType)
        {
            case ItemType.Consumable:
                return CreateConsumableItemById(dataId);

            case ItemType.Weapon:
                return CreateWeaponById(dataId);

            case ItemType.Armor:
                return CreateArmorById(dataId);
        }

        Debug.LogError($"[ItemFactory]Could not create item form id: {dataId}. ItemType is unknown: {itemData.ItemType}.");
        return null;
    }

    public static ConsumableItem CreateConsumableItemById(int dataId)
    {
        if (!_itemDatas.TryGetValue(dataId, out var itemData))
        {
            Debug.LogError($"[ItemFactory]Could not create item form id: {dataId}. No such id is in dictionary.");
            return null;
        }

        var type = Type.GetType(itemData.ClassName);
        if (type != null && typeof(ConsumableItem).IsAssignableFrom(type))
        {
            return (ConsumableItem)Activator.CreateInstance(type, dataId);
        }
        else
        {
            Debug.LogError($"[ItemFactory]Could not create item form id: {dataId}. No such classname exists: {itemData.ClassName}");
            return null;
        }
    }

    public static Weapon CreateWeaponById(int dataId)
    {
        if (!_itemDatas.TryGetValue(dataId, out var itemData))
        {
            Debug.LogError($"[ItemFactory]Could not create item form id: {dataId}. No such id is in dictionary.");
            return null;
        }

        var type = Type.GetType(itemData.ClassName);
        if(type != null && typeof(Weapon).IsAssignableFrom(type))
        {
            return (Weapon)Activator.CreateInstance(type, dataId);
        } 
        else
        {
            return new Weapon(dataId);
        }
    }

    public static Armor CreateArmorById(int dataId)
    {
        if (!_itemDatas.TryGetValue(dataId, out var itemData))
        {
            Debug.LogError($"[ItemFactory]Could not create item form id: {dataId}. No such id is in dictionary.");
            return null;
        }

        var type = Type.GetType(itemData.ClassName);
        if (type != null && typeof(Armor).IsAssignableFrom(type))
        {
            return (Armor)Activator.CreateInstance(type, dataId);
        }
        else
        {
            return new Armor(dataId);
        }
    }
}
