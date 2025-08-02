using System;
using System.Collections.Generic;
using UnityEngine;


public static class ItemFactory
{
    private static Dictionary<int, ItemData> _itemDatas => Managers.DataMng.ItemDataDict;

    /// <summary>
    /// 아이템 dataId로 Item 객체를 생성하여 반환.
    /// </summary>
    public static Item CreateItemById(int dataId)
    {
        if (!_itemDatas.TryGetValue(dataId, out var itemData))
        {
            Debug.LogError($"[ItemFactory]Could not create item from id: {dataId}. No such id is in dictionary.");
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

        Debug.LogError($"[ItemFactory]Could not create item from id: {dataId}. ItemType is unknown: {itemData.ItemType}.");
        return null;
    }

    /// <summary>
    /// 아이템 dataId로 ConsumableItem 객체를 생성하여 반환.
    /// </summary>
    public static ConsumableItem CreateConsumableItemById(int dataId)
    {
        if (!_itemDatas.TryGetValue(dataId, out var itemData))
        {
            Debug.LogError($"[ItemFactory]Could not create item from id: {dataId}. No such id is in dictionary.");
            return null;
        }

        var type = Type.GetType(itemData.ClassName); // 아이템의 클래스 타입을 가져옴
        if (type != null && typeof(ConsumableItem).IsAssignableFrom(type))
        {
            return (ConsumableItem)Activator.CreateInstance(type, dataId);
        }
        else
        {
            Debug.LogError($"[ItemFactory]Could not create ConsumableItem from id: {dataId}. No such classname exists: {itemData.ClassName}");
            return null;
        }
    }

    /// <summary>
    /// 아이템 dataId로 Weapon 객체를 생성하여 반환.
    /// </summary>
    public static Weapon CreateWeaponById(int dataId)
    {
        if (!_itemDatas.TryGetValue(dataId, out var itemData))
        {
            Debug.LogError($"[ItemFactory]Could not create Weapon from id: {dataId}. No such id is in dictionary.");
            return null;
        }

        var type = Type.GetType(itemData.ClassName);
        if (type != null && typeof(Weapon).IsAssignableFrom(type))
        {
            return (Weapon)Activator.CreateInstance(type, dataId);
        }
        else
        {
            return new Weapon(dataId);
        }
    }

    /// <summary>
    /// 아이템 dataId로 Armor 객체를 생성하여 반환.
    /// </summary>
    public static Armor CreateArmorById(int dataId)
    {
        if (!_itemDatas.TryGetValue(dataId, out var itemData))
        {
            Debug.LogError($"[ItemFactory]Could not create Armor from id: {dataId}. No such id is in dictionary.");
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
