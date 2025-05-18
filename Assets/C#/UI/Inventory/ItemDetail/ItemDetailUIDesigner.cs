using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class ItemDetailUIDesigner
{
    private static Dictionary<ItemType, ItemDetailDesign> _designCache = new();

    private static GoldDetailDesign _goldDesign;
    public static void Init()
    {
        _designCache[ItemType.Weapon] = new WeaponItemDetailDesign();
        _designCache[ItemType.Armor] = new ArmorItemDetailDesign();
        _designCache[ItemType.Consumable] = new ConsumableItemDetailDesign();
        _goldDesign = new GoldDetailDesign();
    }

    public static ItemDetailDesign GetDesign(ItemType type)
    {
        return _designCache[type];
    }

    public static GoldDetailDesign GetGoldDesign()
    {
        return _goldDesign;
    }
    
}
