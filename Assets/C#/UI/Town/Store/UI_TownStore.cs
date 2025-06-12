using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_TownStore : UI_Base
{
    enum Buttons
    {
        Button_Close,
    }

    enum ButtonObjects
    {
        Button_BuyPanel,
        Button_SellPanel,
    }

    enum StorePanel
    {
        UI_TownStore_BuyPanel,
        UI_TownStore_SellPanel,
    }

    private StorePanel _activePanel;

    private Dictionary<StorePanel, UI_StorePanelSwitchButton> _panelToButton = new();
    private readonly Dictionary<StorePanel, Type> _tabToUIType = new()
    {
        { StorePanel.UI_TownStore_BuyPanel, typeof(UI_TownStore_BuyPanel) },
        { StorePanel.UI_TownStore_SellPanel, typeof(UI_TownStore_BuyPanel) },
    };

    private RectTransform _rectTransform;
    private float _offscreenY; // 화면에서 UI를 숨길 때 이동할 Y좌표

    public override void Init() { }

    public void LateInit()
    {
        ShowInstantly();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(ButtonObjects));
        Bind<UI_Base>(typeof(StorePanel));
        _rectTransform = GetComponent<RectTransform>();
        _offscreenY = _rectTransform.rect.height;

        GetButton(Buttons.Button_Close).onClick.AddListener(Close);

        foreach (StorePanel panel in Enum.GetValues(typeof(StorePanel)))
        {
            var buttonObj = GetGameObject((ButtonObjects)panel);
            var button = buttonObj.GetOrAddComponent<UI_StorePanelSwitchButton>();
            button.Init();
            button.AddListener(() => ShowPanel(panel));
            _panelToButton[panel] = button;
            button.SetInactive();
        }

        InitPanel(StorePanel.UI_TownStore_BuyPanel);
        InitPanel(StorePanel.UI_TownStore_SellPanel);

        HidePanel(StorePanel.UI_TownStore_SellPanel);
        ShowPanel(StorePanel.UI_TownStore_BuyPanel);
        HideInstantly();
    }

    private void InitPanel(StorePanel storePanel)
    {
        IStorePanel panel = Get<UI_Base>(storePanel) as IStorePanel;
        panel.LateInit();
    }

    private void ShowPanel(StorePanel storePanel)
    {
        // 기존 패널 숨김
        HidePanel(_activePanel);
        var panel = Get<UI_Base>(storePanel);

        panel.ShowInstantly();

        // 현재 패널 버튼을 활성화
        _panelToButton[storePanel].SetActive();
        _activePanel = storePanel;
    }

    private void HidePanel(StorePanel storePanel)
    {
        Get<UI_Base>(storePanel).HideInstantly();
        _panelToButton[storePanel].SetInactive();
    }

    public override Tween Show()
    {
        gameObject.SetActive(true);
        ShowPanel(StorePanel.UI_TownStore_BuyPanel);
        return _rectTransform.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutCubic).OnComplete(() => { GetButton(Buttons.Button_Close).interactable = true; });
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
        Managers.TownMng.UI.CurrentOpenUI = null;
        Hide();
    }
}
