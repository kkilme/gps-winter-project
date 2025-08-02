using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PartyFormation : UI_Base
{
    enum Buttons
    {
        Button_Close,

        Button_Confirm,
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

        _heroDetailParent = GetGameObject(GameObjects.Contents).transform;

        GetButton(Buttons.Button_Close).onClick.AddListener(Close);
        GetButton(Buttons.Button_Confirm).onClick.AddListener(ApplyParty);
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
            if (Managers.HeroMng.IsHeroInParty(heroData))
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

    /// <summary>
    /// UI_SimpleHeroDetail UI가 클릭되었을 때 호출되는 메서드.
    /// </summary>
    public void OnHeroDetailUIClicked(UI_SimpleHeroDetail heroDetailUI)
    {
        if (_selectedHeroDetailUIs.Contains(heroDetailUI))
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
            _selectedHeroDetailUIs[i - 1].SetOrderInParty(i);
        }
    }

    /// <summary>
    /// 선택된 영웅들을 실제로 파티에 적용하는 메서드.
    /// </summary>
    private void ApplyParty()
    {
        if (_selectedHeroDetailUIs.Count == 0) return; // TODO: 선택한 영웅이 없을 시 경고 메시지 표시 등의 처리

        Managers.HeroMng.SetHeroParty(_selectedHeroDetailUIs.ConvertAll(ui => ui.BindingHeroData.InstanceId));
        Managers.TownMng.SpawnHeroes(); // 영웅을 다시 Town에 스폰

        Close();
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
