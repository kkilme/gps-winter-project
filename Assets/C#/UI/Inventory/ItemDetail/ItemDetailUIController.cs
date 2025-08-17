using System.Collections.Generic;
using UnityEngine;


public static class ItemDetailUIController
{
    private static UI_ItemDetailPopup _ui;

    private static Dictionary<ItemType, ItemDetailDesign> _designCache = new();
    private static GoldDetailDesign _goldDesign;

    public static void Init()
    {
        _designCache[ItemType.Weapon] = new WeaponItemDetailDesign();
        _designCache[ItemType.Armor] = new ArmorItemDetailDesign();
        _designCache[ItemType.Consumable] = new ConsumableItemDetailDesign();
        _goldDesign = new GoldDetailDesign();
    }

    public static void HideItemDetailUI()
    {
        if (_ui == null)
        {
            Debug.LogWarning("[ItemDetailUIController] HideItemDetailUI() has been called when _ui is null!");
            return;
        }
        _ui.HideInstantly();
    }

    /// <summary>
    /// ItemInstanceData에 대한 아이템 상세 UI를 보인다.
    /// </summary>
    public static void ShowItemDetailUI(ItemInstanceData itemInstanceData, int itemQuantity = 0)
    {
        if (itemInstanceData == null) return;

        if (_ui == null)
        {
            _ui = Managers.UIMng.ShowPopupUI<UI_ItemDetailPopup>(setSortingOrder: false);
        }

        _ui.ApplyDesign(itemInstanceData, itemQuantity);
        _ui.Show();
    }

    /// <summary>
    /// ItemData에 대한 아이템 상세 UI를 보인다.
    /// </summary>
    public static void ShowItemDetailUI(ItemData itemData, int itemQuantity = 0)
    {
        if (itemData == null) return;

        if (_ui == null)
        {
            _ui = Managers.UIMng.ShowPopupUI<UI_ItemDetailPopup>(setSortingOrder: false);
        }

        _ui.ApplyDesign(itemData, itemQuantity);
        _ui.Show();
    }

    /// <summary>
    /// Gold에 대한 아이템 상세 UI를 보인다.
    /// </summary>
    public static void ShowGoldDetailUI(int goldAmount)
    {
        if (goldAmount <= 0) return;
        if (_ui == null)
        {
            _ui = Managers.UIMng.ShowPopupUI<UI_ItemDetailPopup>(setSortingOrder: false);
        }

        _ui.ApplyGoldDesign(goldAmount);
        _ui.Show();
    }

    // UI_ItemDetailPopup에 디자인을 적용하는 확장 메소드들 /////////
    public static void ApplyDesign(this UI_ItemDetailPopup ui, ItemData itemData, int quantity = 0)
    {
        if (itemData == null) return;
        ItemDetailDesign design = _designCache[itemData.ItemType];
        design.Apply(ui, itemData, quantity);
    }

    public static void ApplyDesign(this UI_ItemDetailPopup ui, ItemInstanceData itemInstanceData, int quantity = 0)
    {
        if (itemInstanceData == null) return;
        ItemDetailDesign design = _designCache[itemInstanceData.ItemType];
        design.Apply(ui, itemInstanceData, quantity);
    }

    public static void ApplyGoldDesign(this UI_ItemDetailPopup ui, int goldAmount)
    {
        if (goldAmount <= 0) return;
        _goldDesign.Apply(ui, itemData: null, goldAmount);
    }
    /////////////////////////////////////////////////////
}
