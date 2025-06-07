using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class ItemDetailUIFactory
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

    public static void CreateItemDetailUI(ItemInstanceData itemInstanceData, int itemQuantity = 0)
    {
        if (itemInstanceData == null) return;

        UI_ItemDetailPopup ui = Managers.UIMng.ShowPopupUI<UI_ItemDetailPopup>();
        ItemDetailDesign design = _designCache[itemInstanceData.ItemType];
        design.Apply(ui, itemInstanceData, itemQuantity);
    }
    public static void CreateItemDetailUI(ItemData itemData, int itemQuantity = 0)
    {
        if (itemData == null) return;

        UI_ItemDetailPopup ui = Managers.UIMng.ShowPopupUI<UI_ItemDetailPopup>();
        ItemDetailDesign design = _designCache[itemData.ItemType];
        design.Apply(ui, itemData, itemQuantity);
    }
}
