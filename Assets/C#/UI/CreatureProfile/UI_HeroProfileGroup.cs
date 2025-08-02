using System;
using UnityEngine;

public class UI_HeroProfileGroup : UI_CreatureProfileGroup
{
    enum HeroProfileUI
    {
        UI_HeroProfile_1,
        UI_HeroProfile_2,
        UI_HeroProfile_3,
        UI_HeroProfile_4, // 현재 UI 최대 4개.
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(HeroProfileUI));
    }

    /// <summary>
    /// 프로필 UI를 파티에 속한 영웅들에게 바인딩.
    /// </summary>
    public override void BindCreature()
    {
        // 먼저 모든 UI를 비활성화
        foreach (HeroProfileUI playerUI in Enum.GetValues(typeof(HeroProfileUI)))
            GetGameObject(playerUI).SetActive(false);

        int index = 0;
        foreach (var kvp in Managers.HeroMng.HeroParty.RuntimeHeroesDict)
        {
            int id = kvp.Key;
            Hero hero = kvp.Value;
            var profile = GetGameObject((HeroProfileUI)index++).GetOrAddComponent<UI_HeroProfile>();

            _creatureProfiles.Add(hero, profile);
            profile.BindHero(Managers.HeroMng.HeroStorage.GetHeroInstanceData(id), hero);
            profile.Show(); // 영웅 수에 맞게 프로필 UI 활성화
        }
    }

    public void OnFlee(Hero hero)
    {
        (_creatureProfiles[hero] as UI_HeroProfile).OnFlee();
    }
}
