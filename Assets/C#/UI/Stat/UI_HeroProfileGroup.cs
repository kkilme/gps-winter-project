using System;
using UnityEngine;

public class UI_HeroProfileGroup : UI_Base
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

    public void BindHeroProfileUIs()
    {
        foreach (HeroProfileUI playerUI in Enum.GetValues(typeof(HeroProfileUI)))
            GetGameObject(playerUI).SetActive(false);

        int index = 0;
        foreach (var hero in Managers.ObjectMng.HeroParty.Heroes)
        {
            var go = GetGameObject((HeroProfileUI)index++);
            go.GetOrAddComponent<UI_CreatureProfile>().BindStat(hero.CreatureStat);
            go.SetActive(true);
            
            //go.transform.Find("Bag").GetOrAddComponent<UI_Bag>().BindBag(hero.Bag);
        }
    }
}
