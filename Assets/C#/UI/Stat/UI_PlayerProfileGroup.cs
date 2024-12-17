using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class UI_PlayerProfileGroup : UI_Base
{
    enum PlayerProfileUI
    {
        UI_PlayerProfile_Player1,
        UI_PlayerProfile_Player2,
        UI_PlayerProfile_Player3,
        UI_PlayerProfile_Player4
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(PlayerProfileUI));
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
            var go = GetGameObject((PlayerProfileUI)index++);
            go.GetOrAddComponent<UI_PlayerProfile>().BindPlayerStat(hero.HeroStat);
            go.SetActive(true);
            
            //go.transform.Find("Bag").GetOrAddComponent<UI_Bag>().BindBag(hero.Bag);
        }
    }

    private void Clear()
    {
        foreach (PlayerProfileUI playerUI in Enum.GetValues(typeof(PlayerProfileUI)))
            GetGameObject(playerUI).SetActive(false);
    }
}
