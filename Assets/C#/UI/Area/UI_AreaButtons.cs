using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UI_AreaButtons : UI_Base
{
    enum Buttons
    {
        Button_Rest,
        Button_LootList,
        Button_ItemList,
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));

        BindEvent(GetButton(Buttons.Button_Rest).gameObject, OnClickRestButton);
        BindEvent(GetButton(Buttons.Button_LootList).gameObject, OnClickLootListButton);
        BindEvent(GetButton(Buttons.Button_ItemList).gameObject, OnClickItemListButton);
    }

    private void OnClickRestButton(PointerEventData evt)
    {
        // TODO: 휴식 기능 호출
    }

    private void OnClickLootListButton(PointerEventData evt)
    {
        //Managers.UIMng.TogglePopupUI<UI_AreaLootList>();
    }

    private void OnClickItemListButton(PointerEventData evt)
    {
        //Managers.UIMng.TogglePopupUI<UI_AreaItemList>();
    }
}
