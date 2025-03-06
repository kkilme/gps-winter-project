using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_CreatureProfile : UI_Base
{
    enum Text
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
    }

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Text));
        Bind<Slider>(typeof(Sliders));
        Bind<Image>(typeof(Images));
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
        
        GetText(Text.Text_Name).text = creatureStat.Name;

        Get<Slider>(Sliders.Slider_HP).value = (float)creatureStat.Hp / creatureStat.MaxHp;
        GetText(Text.Text_HP).text = $"{creatureStat.Hp}/{creatureStat.MaxHp}";
        GetText(Text.Text_BaseDamage).text = creatureStat.BaseDamage.ToString();
        GetText(Text.Text_PhysicalDefense).text = creatureStat.PhysicalDefense.ToString();
        GetText(Text.Text_MagicDefense).text = creatureStat.MagicDefense.ToString();

        GetText(Text.Text_Strength).text = creatureStat.Strength.ToString();
        GetText(Text.Text_Vitality).text = creatureStat.Vitality.ToString();
        GetText(Text.Text_Intelligence).text = creatureStat.Intelligence.ToString();
        GetText(Text.Text_Dexterity).text = creatureStat.Dexterity.ToString();

        Get<Image>(Images.Creature_Image).sprite = Managers.ResourceMng.Load<Sprite>($"Textures/Model_Sprites/{creatureStat.Name}_Front");
    }
}
