using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_HeroList : UI_Base
{
    enum Buttons
    {
        Button_Close,
    }

    enum GameObjects
    {
        Contents,
    }

    private RectTransform _rectTransform;
    private float _offscreenY; // 화면에서 UI를 숨길 때 이동할 Y좌표

    private Transform _heroDetailParent;

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        _rectTransform = GetComponent<RectTransform>();
        _offscreenY = _rectTransform.rect.height;

        GetButton(Buttons.Button_Close).onClick.AddListener(Close);
        _heroDetailParent = GetGameObject(GameObjects.Contents).transform;
    }

    /// <summary>
    /// HeroList UI에 저장된 영웅 데이터를 기반으로 HeroDetail UI를 채움.
    /// </summary>
    private void FillHeroDetailUI()
    {
        List<HeroInstanceData> savedHeroDatas = Managers.HeroMng.GetSavedHeroDatas();
        foreach (var heroData in savedHeroDatas)
        {
            UI_HeroDetail heroDetail = Managers.UIMng.MakeSubItemUI<UI_HeroDetail>(_heroDetailParent, "Town/" + nameof(UI_HeroDetail));
            heroDetail.LateInit(heroData);
        }
    }

    public override Tween Show()
    {
        gameObject.SetActive(true);
        Clear();
        FillHeroDetailUI();
        _heroDetailParent.localPosition = new Vector3(0, 0, 0);
        Canvas.ForceUpdateCanvases(); // scrollbar의 size가 제대로 계산되도록 강제 업데이트
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
        Managers.UIMng.ClosePopupUI<UI_EquipmentSelectWindow>();
        Hide();
    }

    public void Clear()
    {
        for (int i = _heroDetailParent.childCount - 1; i >= 0; i--)
        {
            Destroy(_heroDetailParent.GetChild(i).gameObject);
        }
    }
}
