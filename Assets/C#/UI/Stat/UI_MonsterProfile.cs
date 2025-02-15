using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MonsterProfile : UI_Base
{
    private event Action OnClaer;

    enum Text
    {
        Text_Name,

        Text_HP,
        Text_Attack,
        Text_Defense,
        Text_Speed,
    }

    enum Sliders
    {
        Slider_HP,
    }

    enum Image
    {
        MonsterPicture,
    }

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Text));
        Bind<UnityEngine.UI.Slider>(typeof(Slider));
        Bind<UnityEngine.UI.Image>(typeof(Image));
    }

    public void ConnectPlayerStat(CreatureStat stat)
    {
        stat.StatChangeAction += ChangeMonsterStatUI;

        OnClaer?.Invoke();
        OnClaer = null;
        OnClaer += () => stat.StatChangeAction -= ChangeMonsterStatUI;
    }

    private void ChangeMonsterStatUI(CreatureStat creatureStat)
    {       
        GetText(Text.Text_Name).text = creatureStat.Name;

        Get<Slider>(Sliders.Slider_HP).value = creatureStat.Hp / creatureStat.MaxHp;
        GetText(Text.Text_HP).text = $"{creatureStat.Hp}/{creatureStat.MaxHp}";
        GetText(Text.Text_Attack).text = creatureStat.BaseDamage.ToString();
        GetText(Text.Text_Defense).text = creatureStat.PhysicalDefense.ToString();

        //Get<Image>(Images.UserPicture).sprite = monsterStat.Texture;
    }

    private void OnDestroy()
    {
        OnClaer?.Invoke();
    }
}
