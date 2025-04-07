using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_HeroProfileGroup : UI_CreatureProfileGroup
{
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

    public override void BindProfileUIs()
    {
        foreach (HeroProfileUI playerUI in Enum.GetValues(typeof(HeroProfileUI)))
            GetGameObject(playerUI).SetActive(false);

        int index = 0;
        foreach (var hero in Managers.HeroMng.HeroParty.Heroes)
        {   
            var profile = GetGameObject((HeroProfileUI)index++).GetOrAddComponent<UI_HeroProfile>();
            _creatureProfiles.Add(hero, profile);
            profile.BindCreature(hero);
            profile.Show();
        }
    }

    public void OnFlee(Hero hero)
    {
        (_creatureProfiles[hero] as UI_HeroProfile).OnFlee();
    }
}
