using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// AreaScene에서, Area에 가져온 아이템을 디스플레이 및 사용할 수 있도록 하는 팝업 UI
/// </summary>
public class UI_AreaItemListPopup : UI_Popup
{
    enum GameObjects
    {
        Inventory
    }

    enum Buttons
    {
        Button_Close
    }

    private UI_Inventory _inventory; // 아이템이 담긴 인벤토리

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        GetButton(Buttons.Button_Close).onClick.AddListener(Close);

        UI_Inventory inventory = GetGameObject(GameObjects.Inventory).GetOrAddComponent<UI_Inventory>();
        inventory.LateInit(OnSlotSelected, maxSize: GlobalValues.MAX_AREAITEM_COUNT);
        _inventory = inventory;

        inventory.HardClear();

        foreach (var item in Managers.AreaMng.Items)
        {
            inventory.AddItem(item.ItemData);
        }
    }

    /// <summary>
    /// 유저가 _inventory에서 슬롯을 선택했을 때 호출되는 콜백 함수.
    /// </summary>
    private void OnSlotSelected(UI_ItemSlot selectedSlot)
    {
        if (selectedSlot.ItemData == null) return;

        UI_AreaItemUseConfirmPopup popup = Managers.UIMng.ShowPopupUI<UI_AreaItemUseConfirmPopup>();
        UIUtility.SetRectPositionRelativeTo(selectedSlot.gameObject, popup.Panel.gameObject, UIUtility.RectPosDirection.Left, new Vector2(-3, 0));

        popup.BindItem(selectedSlot.ItemData);
    }

    public override void Close()
    {
        _inventory.ClearActionCallbackOnSlots();
        Managers.UIMng.ClosePopupUI<UI_AreaItemUseConfirmPopup>();
        base.Close();
    }

    private void OnDestroy()
    {
        _inventory.ClearActionCallbackOnSlots();
    }
}
