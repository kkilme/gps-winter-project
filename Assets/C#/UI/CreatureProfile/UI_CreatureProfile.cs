using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CreatureProfile : UI_Base
{
    protected Creature _bindingCreature; // 바인딩된 Creature 인스턴스
    public Creature BindingCreature => _bindingCreature;
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

    protected enum Images
    {
        Creature_Image,
        bg,
        Frame_Stat,
        Frame_Main,
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
    }

    /// <summary>
    /// 바인딩된 Creature의 스탯에 변화가 있을 시 UI 업데이트
    /// </summary>
    protected virtual void UpdateStatProfile(CreatureStat creatureStat)
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

    /// <summary>
    /// 바인딩된 Creature가 피해를 입었을 때 프레임이 빨간색으로 깜빡이게 함
    /// </summary>
    public void OnDamaged()
    {
        _frameColorTweener?.Kill();
        foreach (var image in _frameImages)
        {
            Color originalColor = image.color;
            image.DOColor(Color.red, 0.2f).OnComplete(() =>
            {
                image.DOColor(originalColor, 0.2f);
            });
        }
    }

    /// <summary>
    /// 바인딩된 Creature가 치유를 받았을 때 프레임이 초록색으로 깜빡이게 함
    /// </summary>
    public void OnHeal()
    {
        _frameColorTweener?.Kill();
        foreach (var image in _frameImages)
        {
            Color originalColor = image.color;
            image.DOColor(Color.green, 0.2f).OnComplete(() =>
            {
                image.DOColor(originalColor, 0.2f);
            });
        }
    }

    /// <summary>
    /// 바인딩된 Creature가 사망했을 때의 UI 업데이트
    /// </summary>
    public void OnDead()
    {
        GetText(Texts.Text_Dead).gameObject.SetActive(true);
        _canvasGroup.DOFade(0.33f, 1f).OnComplete(() => GetText(Texts.Text_Dead).gameObject.SetActive(true));
    }

    private void OnDestroy()
    {
        if(_bindingCreature != null)
        {
            var stat = _bindingCreature.CreatureStat;
            stat.OnStatChanged -= UpdateStatProfile;
            _bindingCreature.BindProfileUI(null);
        }
    }
}
