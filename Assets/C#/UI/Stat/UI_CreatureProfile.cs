using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CreatureProfile : UI_Base
{
    private Tweener _blinkTweener;
    private Image _bg;
    private Color _bgOriginalColor;

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
        _bg = Get<Image>(Images.bg);
        _bgOriginalColor = _bg.color;
    }

    public void BindStat(CreatureStat stat)
    {
        stat.StatChangeAction -= UpdateCreatureProfile;
        stat.StatChangeAction += UpdateCreatureProfile;
        
        // init
        UpdateCreatureProfile(stat);
    }

    private void UpdateCreatureProfile(CreatureStat creatureStat)
    {
        GetText(Texts.Text_Name).text = creatureStat.Name;

        Get<Slider>(Sliders.Slider_HP).value = (float)creatureStat.Hp / creatureStat.MaxHp;
        GetText(Texts.Text_HP).text = $"{creatureStat.Hp}/{creatureStat.MaxHp}";
        GetText(Texts.Text_BaseDamage).text = creatureStat.BaseDamage.ToString();
        GetText(Texts.Text_PhysicalDefense).text = creatureStat.PhysicalDefense.ToString();
        GetText(Texts.Text_MagicDefense).text = creatureStat.MagicDefense.ToString();

        GetText(Texts.Text_Strength).text = creatureStat.Strength.ToString();
        GetText(Texts.Text_Vitality).text = creatureStat.Vitality.ToString();
        GetText(Texts.Text_Intelligence).text = creatureStat.Intelligence.ToString();
        GetText(Texts.Text_Dexterity).text = creatureStat.Dexterity.ToString();

        Get<Image>(Images.Creature_Image).sprite = Managers.ResourceMng.Load<Sprite>($"Textures/Model_Sprites/{creatureStat.Name}_Front");
    }

    public void StartBlinking()
    {
        _blinkTweener = _bg.DOColor(Color.yellow, 1f).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopBlinking()
    {
        if (_blinkTweener != null)
            _blinkTweener.Kill();
        _bg.color = _bgOriginalColor;
    }
}
