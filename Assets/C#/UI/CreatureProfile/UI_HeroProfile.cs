using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_HeroProfile : UI_CreatureProfile
{
    enum HeroTexts
    {
        Text_Retreat
    }

    private HeroInstanceData _bindingHeroData;

    public override void Init()
    {
        base.Init();
        Bind<TextMeshProUGUI>(typeof(HeroTexts));
        GetText(Enum.GetNames(typeof(Texts)).Length + HeroTexts.Text_Retreat).gameObject.SetActive(false);
    }

    public void BindHero(HeroInstanceData heroInstanceData, Hero hero)
    {
        _bindingCreature = hero;
        hero.BindProfileUI(this);

        var stat = hero.CreatureStat;
        stat.OnStatChanged -= UpdateStatProfile;
        stat.OnStatChanged += UpdateStatProfile;

        _bindingHeroData = heroInstanceData;

        _bindingHeroData.OnNameChanged -= UpdateName;
        _bindingHeroData.OnNameChanged += UpdateName;
        UpdateName(_bindingHeroData);

        Get<Image>(Images.Creature_Image).sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.CREATURE_IMAGE_PATH_PREFIX + $"{hero.CreatureData.Name}_Front");

        // init
        UpdateStatProfile(stat);
    }

    private void UpdateName(HeroInstanceData heroInstanceData)
    {
        GetText(Texts.Text_Name).text = heroInstanceData.CustomName;
    }

    public void OnFlee()
    {
        _canvasGroup.DOFade(0.33f, 1f).OnComplete(() => GetText(Enum.GetNames(typeof(Texts)).Length + HeroTexts.Text_Retreat).gameObject.SetActive(true));
    }

    private void OnDestroy()
    {
        if(_bindingHeroData != null)
        {
            _bindingHeroData.OnNameChanged -= UpdateName;
        }
    }
}
