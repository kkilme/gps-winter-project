using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// AreaItemListPopup에서 아이템을 선택 시 생성되는, 실제 사용 버튼이 있는 팝업 UI.
/// </summary>
public class UI_AreaItemUseConfirmPopup : UI_Popup
{
    enum Buttons
    {
        Button_Use,
        Button_Cancel
    }

    private ItemData _itemData; // 선택된 아이템 데이터

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        GetButton(Buttons.Button_Cancel).onClick.AddListener(Close);
    }

    public void BindItem(ItemData itemData)
    {
        _itemData = itemData;
        GetButton(Buttons.Button_Use).onClick.AddListener(UseItem);
    }

    private void UseItem()
    {
        if (_itemData == null) return;

        // 아이템 사용
        Managers.AreaMng.UseItem(_itemData);
        Managers.UIMng.ClosePopupUI<UI_AreaItemListPopup>();
        Close();
    }

}
