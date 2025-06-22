using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PartyFormation : UI_Base
{
    enum Buttons
    {
        Button_Close,
    }

    enum GameObjects
    {
        Contents,
    }

    private List<UI_SimpleHeroDetail> _selectedHeroDetailUIs = new List<UI_SimpleHeroDetail>();

    private RectTransform _rectTransform;
    private float _offscreenY; // 화면에서 UI를 숨길 때 이동할 Y좌표

    private Transform _heroDetailParent;

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        _rectTransform = GetComponent<RectTransform>();
        _offscreenY = _rectTransform.rect.height;
        _rectTransform.anchoredPosition = new Vector2(0, _offscreenY);

        GetButton(Buttons.Button_Close).onClick.AddListener(Close);
        _heroDetailParent = GetGameObject(GameObjects.Contents).transform;
    }

    /// <summary>
    /// 보유한 모든 영웅의 데이터를 기반으로 SimpleHeroDetail UI를 생성 및 초기화하고 GridLayout에 채움.
    /// </summary>
    private void FillSimpleHeroDetailUI()
    {
        List<HeroInstanceData> savedHeroDatas = Managers.HeroMng.HeroStorage.GetAllOwnedHeroDatas();
        foreach (var heroData in savedHeroDatas)
        {
            UI_SimpleHeroDetail heroDetail = Managers.UIMng.MakeSubItemUI<UI_SimpleHeroDetail>(_heroDetailParent, "Town/" + nameof(UI_SimpleHeroDetail));
            heroDetail.LateInit(this, heroData);

            // 파티에 속해 있는 영웅인지 확인하고, UI에 반영
            if (Managers.HeroMng.HeroParty.ContainsHero(heroData.InstanceId))
            {
                _selectedHeroDetailUIs.Add(heroDetail);
            }
            else
            {
                heroDetail.DisableOrderInParty();
            }
            UpdateHeroDetailUIOrder();
        }
    }

    public void OnHeroDetailUIClicked(UI_SimpleHeroDetail heroDetailUI)
    {
        if(_selectedHeroDetailUIs.Contains(heroDetailUI))
        {
            // 이미 선택된 영웅을 클릭한 경우, 선택 해제
            _selectedHeroDetailUIs.Remove(heroDetailUI);
            heroDetailUI.DisableOrderInParty();
        }
        else if (_selectedHeroDetailUIs.Count < GlobalValues.MAX_PARTY_SIZE)
        {
            // 새 영웅을 선택한 경우, 선택된 영웅 목록에 추가
            _selectedHeroDetailUIs.Add(heroDetailUI);
        }
        UpdateHeroDetailUIOrder();
    }

    private void UpdateHeroDetailUIOrder()
    {
        for (int i = 1; i <= _selectedHeroDetailUIs.Count; i++)
        {
            _selectedHeroDetailUIs[i-1].SetOrderInParty(i);
        }
    }

    public override Tween Show()
    {
        gameObject.SetActive(true);
        Clear();
        FillSimpleHeroDetailUI();
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
        Managers.UIMng.ClosePopupUI<UI_EquipmentSelectPopup>();
        Hide();
    }

    public void Clear()
    {
        _selectedHeroDetailUIs.Clear();
        for (int i = _heroDetailParent.childCount - 1; i >= 0; i--)
        {
            Destroy(_heroDetailParent.GetChild(i).gameObject);
        }
    }
}
