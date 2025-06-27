using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 퀘스트 Area에 가져갈 아이템 선택에 사용되는 인벤토리형 팝업 UI.
/// </summary>
public class UI_QuestItemSelectPopup : UI_Popup
{
    enum GameObjects
    {
        Inventory
    }

    enum Buttons
    {
        Button_Close
    }

    public UI_Inventory PopupInventory { get; set; } // 가져갈 수 있는 아이템이 담긴 인벤토리 (이 팝업창의 인벤토리)
    private UI_QuestDetailPanel _detailPanel;

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        GetButton(Buttons.Button_Close).onClick.AddListener(Close);
    }

    /// <summary>
    /// 아이템 선택용 인벤토리 창을 초기화.
    /// </summary>
    public void LateInit(UI_QuestDetailPanel detailPanel)
    {
        UI_Inventory inventory = GetGameObject(GameObjects.Inventory).GetOrAddComponent<UI_Inventory>();
        inventory.LateInit(OnSlotSelected);
        PopupInventory = inventory;
        _detailPanel = detailPanel;

        inventory.HardClear();

        List<ItemInstanceData> items = Managers.InvMng.GetAllItemsOfType(ItemType.Consumable); // 현재 Area에 가져갈 수 있는 아이템은 소모품 뿐
        foreach (var item in items)
        {
            inventory.AddItem(item.ItemData, item.Quantity);
        }

        foreach (var slot in detailPanel.ItemInventory.InventorySlots)
        {
            inventory.RemoveItem(slot.ItemData); // 이미 유저가 가져가고자 선택한 아이템 제거
        }

        ScrollRect scrollRect = GetComponentInChildren<ScrollRect>();
        scrollRect.verticalNormalizedPosition = 1f; // 스크롤을 맨 위로 이동
    }

    /// <summary>
    /// 유저가 _popupInventory에서 슬롯을 선택했을 때 호출되는 콜백 함수.
    /// </summary>
    private void OnSlotSelected(UI_ItemSlot selectedSlot)
    {
        if (selectedSlot.ItemData == null || _detailPanel.ItemInventory.IsFull()) return;

        PopupInventory.RemoveItem(selectedSlot, quantity: 1);
        _detailPanel.AddItem(selectedSlot.ItemData);
    }

    public override void Close()
    {
        PopupInventory.ClearActionCallbackOnSlots();
        base.Close();
    }

    private void OnDestroy()
    {
        PopupInventory.ClearActionCallbackOnSlots();
    }
}
