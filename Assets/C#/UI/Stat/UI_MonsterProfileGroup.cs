using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_MonsterProfileGroup : UI_CreatureProfileGroup
{
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

    public override void BindProfileUIs()
    {
        foreach (MonsterProfileUI monsterUI in Enum.GetValues(typeof(MonsterProfileUI)))
            GetGameObject(monsterUI).SetActive(false);

        int index = 0;

        foreach (var monster in Managers.BattleMng.Monsters)
        {
            var profile = GetGameObject((MonsterProfileUI)index++).GetOrAddComponent<UI_MonsterProfile>();
            _creatureProfiles.Add(monster, profile);
            profile.BindStat(monster.CreatureStat);
            profile.Show();
        }
    }
}
