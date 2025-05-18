using DG.Tweening;
using System.Collections;
using TMPro;
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

    enum Texts
    {
        Text_RestCount,
    }

    private AreaManager _areaManager => Managers.AreaMng;

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));

        GetButton(Buttons.Button_Rest).onClick.AddListener(OnClickRestButton);
        GetButton(Buttons.Button_LootList).onClick.AddListener(OnClickLootListButton);
        GetButton(Buttons.Button_ItemList).onClick.AddListener(OnClickItemListButton);
    }

    public override Tween Show()
    {
        GetText(Texts.Text_RestCount).text = Managers.AreaMng.AreaData.MaxRestCount.ToString();
        return base.Show();
    }

    private void OnClickRestButton()
    {
        if (_areaManager.AreaState != AreaState.Idle) return;

        int restCount = int.Parse(GetText(Texts.Text_RestCount).text);

        CoroutineRunner.Instance.StartCoroutine(_areaManager.RestParty());

        restCount -= 1;
        GetText(Texts.Text_RestCount).text = restCount.ToString();

        if (restCount == 0)
        {
            GetButton(Buttons.Button_Rest).interactable = false;
            ClearEvent(GetButton(Buttons.Button_Rest).gameObject);
        }
    }

    private void OnClickLootListButton()
    {
        //Managers.UIMng.TogglePopupUI<UI_AreaLootList>();
    }

    private void OnClickItemListButton()
    {
        //Managers.UIMng.TogglePopupUI<UI_AreaItemList>();
    }
}
