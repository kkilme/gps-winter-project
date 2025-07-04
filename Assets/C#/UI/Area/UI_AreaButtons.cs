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
    private UI_Popup _openPopup; // LootList와 ItemList 중 현재 열려있는 팝업 UI

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
        GetText(Texts.Text_RestCount).text = _areaManager.AreaData.MaxRestCount.ToString();
        return base.Show();
    }

    /// <summary>
    /// 회복 버튼 누를 시의 콜백 함수. 영웅 파티를 회복시키고 회복 횟수를 감소시킨다.
    /// </summary>
    private void OnClickRestButton()
    {
        if (_areaManager.AreaState != AreaState.Idle) return;

        int restCount = int.Parse(GetText(Texts.Text_RestCount).text);

        // 회복
        CoroutineRunner.Instance.StartCoroutine(_areaManager.RestParty());

        // 남은 회복 횟수 감소
        restCount -= 1;
        GetText(Texts.Text_RestCount).text = restCount.ToString();

        // 회복 횟수가 0이 되면 버튼 비활성화
        if (restCount == 0)
        {
            GetButton(Buttons.Button_Rest).interactable = false;
            ClearEvent(GetButton(Buttons.Button_Rest).gameObject);
        }
    }

    /// <summary>
    /// 전리품(Loots) 버튼을 누를 시의 콜백 함수. Area에서 획득한 전리품 목록을 표시하는 팝업 UI를 열고 적절히 위치를 설정한다.
    /// </summary>
    private void OnClickLootListButton()
    {
        if (_openPopup != null && _openPopup is not UI_AreaLootListPopup) _openPopup.Close();

        UI_AreaLootListPopup popup = Managers.UIMng.TogglePopupUI<UI_AreaLootListPopup>();

        if (popup == null) return;

        _openPopup = popup;
        UIUtility.SetRectPositionRelativeTo(
            GetButton(Buttons.Button_LootList).gameObject,
            popup.Panel.gameObject,
            UIUtility.RectPosDirection.Left,
            new Vector2(-10, 0)
            );
    }

    /// <summary>
    /// 아이템(Items) 버튼을 누를 시의 콜백 함수. Area에 가져온 아이템 목록을 표시하는 팝업 UI를 열고 적절히 위치를 설정한다.
    /// </summary>
    private void OnClickItemListButton()
    {
        if(_openPopup != null && _openPopup is not UI_AreaItemListPopup) _openPopup.Close();

        UI_AreaItemListPopup popup = Managers.UIMng.TogglePopupUI<UI_AreaItemListPopup>();

        if(popup == null) return;

        _openPopup = popup;
        UIUtility.SetRectPositionRelativeTo(
            GetButton(Buttons.Button_ItemList).gameObject,
            popup.Panel.gameObject,
            UIUtility.RectPosDirection.Left,
            new Vector2(-10, 0)
            );
    }
}
