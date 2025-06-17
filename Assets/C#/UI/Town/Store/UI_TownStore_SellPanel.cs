using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Town Store에서 판매 패널을 나타냄.
/// </summary>
public class UI_TownStore_SellPanel : UI_Base, IStorePanel
{
    enum GameObjects
    {
        Content_Inventory,
        UI_SellConfirmPanel,
    }

    private UI_Inventory _inventory; // 판매할 아이템을 담는 인벤토리 UI
    private UI_SellConfirmPanel _sellConfirmPanel; // 판매 개수 설정 및 실제 판매 버튼이 있는 패널

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        _inventory = GetGameObject(GameObjects.Content_Inventory).GetOrAddComponent<UI_Inventory>();
        _sellConfirmPanel = GetGameObject(GameObjects.UI_SellConfirmPanel).GetOrAddComponent<UI_SellConfirmPanel>();
        _sellConfirmPanel.OnSell -= ShowInventory;
        _sellConfirmPanel.OnSell += ShowInventory;
    }

    public void LateInit()
    {
        _inventory.LateInit(ShowSellPanel);
        _sellConfirmPanel.LateInit();
    }

    /// <summary>
    /// 판매 가능한 아이템으로 인벤토리를 채움.
    /// </summary>
    private void ShowInventory()
    {
        _inventory.Clear();
        _inventory.ShowInstantly();

        List<ItemInstanceData> allItems = Managers.InvMng.ItemList;

        foreach (ItemInstanceData item in allItems)
        {
            if (item is EquipmentInstanceData equipmentInstanceData && equipmentInstanceData.IsEquipped) continue; // 장착 중인 장비는 판매 불가. 처음부터 판매용 인벤토리에 표시하지 않음.
            _inventory.AddItem(item);
        }

        // TODO?: 인벤토리 정렬 기능

        _sellConfirmPanel.HideInstantly();
    }

    private void ShowSellPanel(UI_InventorySlot selectedSlot)
    {
        if(selectedSlot.IsEmpty) return;

        _sellConfirmPanel.ShowInstantly();
        _sellConfirmPanel.BindItem(selectedSlot.ItemInstanceData);
    }

    public override void ShowInstantly()
    {
        base.ShowInstantly();
        ShowInventory();
    }
}
