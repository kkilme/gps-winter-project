using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// BattleScene에서 아이템을 디스플레이 및 사용할 수 있도록 하는 UI. AreaItemListPopup과 매우 유사함.
/// </summary>
public class UI_BattleBag : UI_Base
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
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        GetButton(Buttons.Button_Close).onClick.AddListener(HideInstantly);
        _inventory = GetGameObject(GameObjects.Inventory).GetOrAddComponent<UI_Inventory>();
        _inventory.LateInit(OnSlotSelected, maxSize: GlobalValues.MAX_AREAITEM_COUNT);
    }

    public override void ShowInstantly()
    {
        _inventory.HardClear();

        foreach (var item in Managers.AreaMng.Items)
        {
            _inventory.AddItem(item.ItemData);
        }
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 유저가 _inventory에서 슬롯을 선택했을 때 호출되는 콜백 함수.
    /// </summary>
    private void OnSlotSelected(UI_ItemSlot selectedSlot)
    {
        if (selectedSlot.ItemData == null) return;
        Managers.UIMng.ClosePopupUI<UI_ConfirmActionPopup>(); // 이미 열린 팝업 있다면 닫기
        UI_ConfirmActionPopup popup = Managers.UIMng.ShowPopupUI<UI_ConfirmActionPopup>();
        UIUtility.SetRectPositionRelativeTo(selectedSlot.gameObject, popup.Panel.gameObject, UIUtility.RectPosDirection.Left); // 팝업 위치 설정

        ItemData itemData = selectedSlot.ItemData;
        popup.SetTitle(itemData.Name);
        popup.SetConfirmText("Use");
        popup.SetConfirmAction(UseItem);

        void UseItem()
        {
            List<Item> currentItems = Managers.AreaMng.Items;
            Item item = currentItems.Find(i => i.DataId == itemData.DataId);
            if (item == null) return; // 아이템이 존재하는지 확인
            if(item is not IUsableInBattle battleItem) return; // Battle에서 사용 가능한 아이템인지 확인

            ItemAction itemAction = battleItem.GetItemAction(); // 아이템 액션 가져오기
            HideInstantly(); // UnsetAction부터 하기 위해 먼저 해야 함
            Managers.BattleMng.SetAction(itemAction); // 현재 액션으로 설정
        }
    }

    public override void HideInstantly()
    {
        Managers.BattleMng.UnsetAction();
        Managers.UIMng.ClosePopupUI<UI_ConfirmActionPopup>();
        base.HideInstantly();
    }
}
