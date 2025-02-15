using System;
using System.Collections;
using Unity.VisualScripting;
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
        //DontDestroyOnLoad(transform.parent.gameObject);
    }

    void Start()
    {
        Clear();
        BindPlayerUIs();
    }

    private void BindPlayerUIs()
    {
        int index = 0;
        foreach (var hero in Managers.ObjectMng.Heroes.Values)
        {
            var go = GetGameObject((HeroProfileUI)index++);
            go.GetOrAddComponent<UI_CreatureProfile>().BindStat(hero.CreatureStat);
            go.SetActive(true);
            
            //go.transform.Find("Bag").GetOrAddComponent<UI_Bag>().BindBag(hero.Bag);
        }
    }

    private void Clear()
    {
        foreach (HeroProfileUI playerUI in Enum.GetValues(typeof(HeroProfileUI)))
            GetGameObject(playerUI).SetActive(false);
    }
}
