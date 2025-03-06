using System;
using UnityEngine;

public class UI_MonsterProfileGroup : UI_Base
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

    public void BindMonsterProfileUIs()
    {
        foreach (MonsterProfileUI monsterUI in Enum.GetValues(typeof(MonsterProfileUI)))
            GetGameObject(monsterUI).SetActive(false);

        int index = 0;

        foreach (var monster in Managers.BattleMng.Monsters)
        {
            var go = GetGameObject((MonsterProfileUI)index++);
            go.GetOrAddComponent<UI_CreatureProfile>().BindStat(monster.CreatureStat);
            go.SetActive(true);
        }
    }
}
