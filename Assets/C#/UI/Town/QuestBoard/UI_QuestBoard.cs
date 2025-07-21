using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Town에서 퀘스트를 관리하는 UI. 퀘스트 목록과 상세 정보를 보여주는 패널을 포함.
/// </summary>
public class UI_QuestBoard : UI_Base
{   
    enum Panels
    {
        UI_QuestList,
        UI_QuestDetailPanel,
    }

    enum Buttons
    {
        Button_Close
    }

    private UI_QuestList _questList;
    private UI_QuestDetailPanel _questDetailPanel;

    private RectTransform _rectTransform;
    private float _offscreenY; // 화면에서 UI를 숨길 때 이동할 Y좌표

    public override void Init() {}

    public void LateInit()
    {
        ShowInstantly();
        Bind<GameObject>(typeof(Panels));
        Bind<Button>(typeof(Buttons));

        _questList = GetGameObject(Panels.UI_QuestList).GetOrAddComponent<UI_QuestList>();
        _questList.LateInit(this);
        _questDetailPanel = GetGameObject(Panels.UI_QuestDetailPanel).GetOrAddComponent<UI_QuestDetailPanel>();
        _questDetailPanel.LateInit();
        _rectTransform = GetComponent<RectTransform>();
        _offscreenY = _rectTransform.rect.height;

        HideInstantly();
    }

    public void ShowQuestDetailPanel(Quest quest)
    {
        _questDetailPanel.BindQuest(quest);
    }

    public override Tween Show()
    {
        gameObject.SetActive(true);
        _questDetailPanel.HideInstantly();
        _questList.Show();
        return _rectTransform.DOAnchorPosY(0, 0.5f).
            SetEase(Ease.OutCubic).
            OnComplete(() => { GetButton(Buttons.Button_Close).interactable = true; });
    }

    public override Tween Hide()
    {
        GetButton(Buttons.Button_Close).interactable = false;
        return _rectTransform.DOAnchorPosY(_offscreenY, 0.5f)
            .SetEase(Ease.InCubic)
            .OnComplete(() => gameObject.SetActive(false));
    }

    public override void HideInstantly()
    {
        _rectTransform.DOAnchorPosY(_offscreenY, 0f);
        base.HideInstantly();
    }

    public void Close()
    {
        Managers.UIMng.ClosePopupUI<UI_QuestItemSelectPopup>();
        Managers.TownMng.UI.CurrentOpenUI = null;
        Hide();
    }
}
