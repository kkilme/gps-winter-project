using System.Collections.Generic;


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

    /// <summary>
    /// ItemInstanceData에 대한 아이템 상세 UI를 생성한다.
    /// </summary>
    public static void CreateItemDetailUI(ItemInstanceData itemInstanceData, int itemQuantity = 0)
    {
        if (itemInstanceData == null) return;

        UI_ItemDetailPopup ui = Managers.UIMng.ShowPopupUI<UI_ItemDetailPopup>();
        ui.ApplyDesign(itemInstanceData, itemQuantity);
    }

    /// <summary>
    /// ItemData에 대한 아이템 상세 UI를 생성한다.
    /// </summary>
    public static void CreateItemDetailUI(ItemData itemData, int itemQuantity = 0)
    {
        if (itemData == null) return;

        UI_ItemDetailPopup ui = Managers.UIMng.ShowPopupUI<UI_ItemDetailPopup>();
        ui.ApplyDesign(itemData, itemQuantity);
    }

    /// <summary>
    /// Gold에 대한 아이템 상세 UI를 생성한다.
    /// </summary>
    public static void CreateGoldDetailUI(int goldAmount)
    {
        if (goldAmount <= 0) return;
        UI_ItemDetailPopup ui = Managers.UIMng.ShowPopupUI<UI_ItemDetailPopup>();
        ui.ApplyGoldDesign(goldAmount);
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
