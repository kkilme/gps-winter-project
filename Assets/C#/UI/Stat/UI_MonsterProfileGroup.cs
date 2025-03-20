using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_MonsterProfileGroup : UI_Base
{
    private Dictionary<Monster, UI_CreatureProfile> _montserProfiles = new Dictionary<Monster, UI_CreatureProfile>();
    enum MonsterProfileUI
    {
        UI_MonsterProfile_1,
        UI_MonsterProfile_2,
        UI_MonsterProfile_3,
        UI_MonsterProfile_4,
        UI_MonsterProfile_5,
        UI_MonsterProfile_6,
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(MonsterProfileUI));
    }

    public void BindMonsterProfileUIs()
    {
        foreach (MonsterProfileUI monsterUI in Enum.GetValues(typeof(MonsterProfileUI)))
            GetGameObject(monsterUI).SetActive(false);

        int index = 0;

        foreach (var monster in Managers.BattleMng.Monsters)
        {
            var profile = GetGameObject((MonsterProfileUI)index++).GetOrAddComponent<UI_CreatureProfile>();
            _montserProfiles.Add(monster, profile);
            profile.BindStat(monster.CreatureStat);
            profile.Show();
        }
    }

    public void StopBlinking()
    {
        foreach (var profile in _montserProfiles.Values)
            profile.StopBlinking();
    }

    public void StartBlinking(Monster monster)
    {
        _montserProfiles[monster].StartBlinking();
    }
}
