using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_HeroProfileGroup : UI_Base
{
    private Dictionary<Hero, UI_CreatureProfile> _heroProfiles = new Dictionary<Hero, UI_CreatureProfile>();

    enum HeroProfileUI
    {
        UI_HeroProfile_1,
        UI_HeroProfile_2,
        UI_HeroProfile_3,
        UI_HeroProfile_4,
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(HeroProfileUI));
    }

    public void BindHeroProfileUIs()
    {
        foreach (HeroProfileUI playerUI in Enum.GetValues(typeof(HeroProfileUI)))
            GetGameObject(playerUI).SetActive(false);

        int index = 0;
        foreach (var hero in Managers.ObjectMng.HeroParty.Heroes)
        {   
            var profile = GetGameObject((HeroProfileUI)index++).GetOrAddComponent<UI_CreatureProfile>();
            _heroProfiles.Add(hero, profile);
            profile.BindStat(hero.CreatureStat);
            profile.Show();
        }
    }

    public void StopBlinking()
    {
        foreach (var profile in _heroProfiles.Values)
            profile.StopBlinking();
    }

    public void StartBlinking(Hero hero)
    {
        _heroProfiles[hero].StartBlinking();
    }
}
