using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CreatureProfile : UI_Base
{
    protected CanvasGroup _canvasGroup;
    private Tweener _blinkTweener;
    private Image _bg;
    private Color _bgOriginalColor;
    private Creature _creature;

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
    }

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Slider>(typeof(Sliders));
        Bind<Image>(typeof(Images));
        GetText(Texts.Text_Dead).gameObject.SetActive(false);
        _canvasGroup = GetComponent<CanvasGroup>();
        _bg = Get<Image>(Images.bg);
        _bgOriginalColor = _bg.color;
    }

    public void BindCreature(Creature creature)
    {
        _creature = creature;

        var stat = creature.CreatureStat;
        stat.OnStatChanged -= UpdateStatProfile;
        stat.OnStatChanged += UpdateStatProfile;

        GetText(Texts.Text_Name).text = creature.CreatureData.Name;
        Get<Image>(Images.Creature_Image).sprite = Managers.ResourceMng.Load<Sprite>($"Textures/Model_Sprites/{creature.CreatureData.Name}_Front");

        // init
        UpdateStatProfile(stat);
    }

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

    public void StartBlinking()
    {
        if (_blinkTweener != null)
            _blinkTweener.Kill();
        _blinkTweener = _bg.DOColor(Color.yellow, 1f).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopBlinking()
    {
        if (_blinkTweener != null)
            _blinkTweener.Kill();
        _bg.color = _bgOriginalColor;
    }

    public void OnDead()
    {
        GetText(Texts.Text_Dead).gameObject.SetActive(true);
        _canvasGroup.DOFade(0.33f, 1f).OnComplete(() => GetText(Texts.Text_Dead).gameObject.SetActive(true));
    }
}
