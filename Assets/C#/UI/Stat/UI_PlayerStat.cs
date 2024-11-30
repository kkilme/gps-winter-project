using System;
using TMPro;
using UnityEngine.UI;

public class UI_PlayerStat : UI_Base
{
    private event Action OnClaer;

    enum Text
    {
        Text_Name,

        Text_HP,
        Text_Attack,
        Text_Defense,
        Text_Speed,

        Text_Strength,
        Text_Vitality,
        Text_Intelligence,
        Text_Dexterity,

        Text_Gold,
    }

    enum Sliders
    {
        Slider_HP,
    }

    enum Image
    {
        UserPicture,
    }

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Text));
        Bind<UnityEngine.UI.Slider>(typeof(Sliders));
        Bind<UnityEngine.UI.Image>(typeof(Image));
    }

    public void BindPlayerStat(HeroStat stat)
    {
        stat.StatChangeAction += ChangePlayerStatUI;

        OnClaer?.Invoke();
        OnClaer = null;
        OnClaer += () => stat.StatChangeAction -= ChangePlayerStatUI;
        
        // init
        ChangePlayerStatUI(stat);
    }

    private void ChangePlayerStatUI(CreatureStat creatureStat)
    {
        HeroStat heroStat = (HeroStat)creatureStat;
        
        GetText(Text.Text_Name).text = heroStat.Name;

        Get<Slider>(Sliders.Slider_HP).value = (float)heroStat.Hp / heroStat.MaxHp;
        GetText(Text.Text_HP).text = $"{heroStat.Hp}/{heroStat.MaxHp}";
        GetText(Text.Text_Attack).text = heroStat.Attack.ToString();
        GetText(Text.Text_Defense).text = heroStat.Defense.ToString();

        GetText(Text.Text_Strength).text = heroStat.Strength.ToString();
        GetText(Text.Text_Vitality).text = heroStat.Vitality.ToString();
        GetText(Text.Text_Intelligence).text = heroStat.Intelligence.ToString();
        GetText(Text.Text_Dexterity).text = heroStat.Dexterity.ToString();
        //GetText(Texts.Text_Gold).text = heroStat.Gold.ToString();
        //Get<Image>(Images.UserPicture).sprite = heroStat.Texture;
    }

    private void OnDestroy()
    {
        OnClaer?.Invoke();
    }
}
