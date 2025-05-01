using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CreatureProfile : UI_Base
{
    protected CanvasGroup _canvasGroup;

    private Tweener _blinkTweener;
    private Tweener _frameColorTweener;

    private Image _creatureImageBg;
    private Color _creatureImageBgOriginalColor;
    private List<Image> _frameImages = new List<Image>();

    protected enum Texts
    {
        Text_Name,

        Text_HP,
        Text_BaseDamage,
        Text_PhysicalDefense,
        Text_MagicDefense,

        Text_Strength,
        Text_Vitality,
        Text_Intelligence,
        Text_Dexterity,

        Text_Dead,
    }

    enum Sliders
    {
        Slider_HP,
    }

    enum Images
    {
        Creature_Image,
        bg,
        Frame_Stat,
        Frame_Main,
        Frame_Creature_Image,
    }

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Slider>(typeof(Sliders));
        Bind<Image>(typeof(Images));
        GetText(Texts.Text_Dead).gameObject.SetActive(false);
        _canvasGroup = GetComponent<CanvasGroup>();
        _creatureImageBg = Get<Image>(Images.bg);
        _creatureImageBgOriginalColor = _creatureImageBg.color;

        _frameImages.Add(Get<Image>(Images.Frame_Stat));
        _frameImages.Add(Get<Image>(Images.Frame_Main));
        _frameImages.Add(Get<Image>(Images.Frame_Creature_Image));
    }

    /// <summary>
    /// Creature를 UI에 바인딩
    /// </summary>
    public void BindCreature(Creature creature)
    {
        creature.BindProfileUI(this);

        var stat = creature.CreatureStat;
        stat.OnStatChanged -= UpdateStatProfile;
        stat.OnStatChanged += UpdateStatProfile;

        GetText(Texts.Text_Name).text = creature.CreatureData.Name;
        Get<Image>(Images.Creature_Image).sprite = Managers.ResourceMng.Load<Sprite>($"Textures/Model_Sprites/{creature.CreatureData.Name}_Front");

        // init
        UpdateStatProfile(stat);
    }

    /// <summary>
    /// 바인딩된 Creature의 스탯에 변화가 있을 시 UI 업데이트
    /// </summary>
    private void UpdateStatProfile(CreatureStat creatureStat)
    {
        var stats = new (Texts, int)[]
        {
            (Texts.Text_BaseDamage, creatureStat.BaseDamage),
            (Texts.Text_PhysicalDefense, creatureStat.PhysicalDefense),
            (Texts.Text_MagicDefense, creatureStat.MagicDefense),
            (Texts.Text_Strength, creatureStat.Strength),
            (Texts.Text_Vitality, creatureStat.Vitality),
            (Texts.Text_Intelligence, creatureStat.Intelligence),
            (Texts.Text_Dexterity, creatureStat.Dexterity),
        };
        GetText(Texts.Text_HP).text = $"{creatureStat.Hp}/{creatureStat.MaxHp}";
        Get<Slider>(Sliders.Slider_HP).value = (float)creatureStat.Hp / creatureStat.MaxHp;

        foreach (var (textType, value) in stats)
        {
            GetText(textType).text = value.ToString();
        }

    }

    /// <summary>
    /// Battle에서 현재 턴인 Creature일 시 깜빡임 효과 재생
    /// </summary>
    public void StartBlinking()
    {
        _blinkTweener?.Kill();
        _blinkTweener = _creatureImageBg.DOColor(Color.yellow, 1f).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopBlinking()
    {
        _blinkTweener?.Kill();
        _creatureImageBg.color = _creatureImageBgOriginalColor;
    }

    public void OnDamaged()
    {
        _frameColorTweener?.Kill();
        foreach (var image in _frameImages)
        {
            image.DOColor(Color.red, 0.2f).OnComplete(() =>
            {
                image.DOColor(Color.white, 0.2f);
            });
        }
    }

    public void OnHeal()
    {
        _frameColorTweener?.Kill();
        foreach (var image in _frameImages)
        {
            image.DOColor(Color.green, 0.2f).OnComplete(() =>
            {
                image.DOColor(Color.white, 0.2f);
            });
        }
    }

    public void OnDead()
    {
        GetText(Texts.Text_Dead).gameObject.SetActive(true);
        _canvasGroup.DOFade(0.33f, 1f).OnComplete(() => GetText(Texts.Text_Dead).gameObject.SetActive(true));
    }
}
