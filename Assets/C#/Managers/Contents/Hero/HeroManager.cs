using System.Collections.Generic;
using UnityEngine;

public class HeroManager
{
    public HeroParty HeroParty { get; private set; } = new HeroParty();
    public HeroStorage HeroStorage { get; private set; } = new HeroStorage();
    private Transform _heroRoot => GlobalUtility.FindOrCreateTransform("@Heroes");

    /// <summary>
    /// 현재 파티에 편성된 영웅들을 게임오브젝트로 스폰. 오브젝트의 Transform 설정은 별도로 해주어야 함에 주의.
    /// </summary>
    public void SpawnHeroParty()
    {
        DestroyHeroParty();
        foreach (int heroInstanceId in HeroParty.HeroIds)
        {
            HeroInstanceData data = HeroStorage.GetHeroInstanceData(heroInstanceId);
            if (data != null)
            {
                HeroStat stat = data.Stat;

                if (stat != null)
                {
                    stat.ClearBuffAndDebuffStats();
                    GameObject go = Managers.ResourceMng.Instantiate(GlobalValues.HERO_PREFAB_PATH_PREFIX + data.ClassName, _heroRoot);

                    Hero hero = go.GetOrAddComponent<Hero>();

                    // 저장된 스탯 적용
                    hero.SetData(data.HeroDataId, stat);

                    // 저장된 무기 장착
                    int weaponDataId = HeroStorage.GetEquippedWeapon(heroInstanceId)?.ItemDataId ?? -1;
                    if (weaponDataId != -1)
                        hero.EquipWeapon(weaponDataId);
                    else
                        hero.EquipWeapon(GlobalValues.HERO_HANDWEAPON_ID);

                    // 저장된 방어구 장착
                    var equippedArmors = HeroStorage.GetEquippedArmors(heroInstanceId);
                    if (equippedArmors != null)
                    {
                        foreach (var armors in equippedArmors)
                        {
                            if (armors.Value == null) continue;
                            Armor armor = new Armor(armors.Value.ItemDataId);
                            hero.EquipArmor(armor);
                        }
                    }

                    HeroParty.AddRuntimeHero(heroInstanceId, hero);
                }
            }
        }
    }

    /// <summary>
    /// 게임 오브젝트로 스폰된 영웅들을 파괴하고, HeroParty의 런타임 영웅 리스트를 초기화.
    /// </summary>
    public void DestroyHeroParty()
    {
        foreach (var hero in HeroParty.RuntimeHeroes)
        {
            if (hero != null)
            {
                Managers.ResourceMng.Destroy(hero.gameObject);
            }
        }
        HeroParty.ClearRuntimeHeroes();
    }

    /// <summary>
    /// 영웅 인스턴스 ID 리스트를 받아 파티로 설정.
    /// </summary>
    /// <remarks>
    /// 리스트의 크기는 1 이상, GlobalValues.MAX_PARTY_SIZE 이하여야한다. 영웅 스폰은 따로 해주어야 한다.
    /// </remarks>
    public void SetHeroParty(List<int> heroInstanceIds)
    {
        if (heroInstanceIds == null || heroInstanceIds.Count == 0)
        {
            Debug.LogWarning("[HeroManager] Cannot set empty hero party.");
            return;
        }

        if (heroInstanceIds.Count > GlobalValues.MAX_PARTY_SIZE)
        {
            Debug.LogWarning($"[HeroManager] Cannot set hero party with more than {GlobalValues.MAX_PARTY_SIZE} heroes.");
            return;
        }

        DestroyHeroParty();
        HeroParty.HeroIds.Clear();

        foreach (int heroInstanceId in heroInstanceIds)
        {
            HeroParty.AddHero(heroInstanceId);
        }
    }

    public bool IsHeroInParty(int heroInstanceId)
    {
        return HeroParty.HeroIds.Contains(heroInstanceId);
    }

    public bool IsHeroInParty(HeroInstanceData heroInstanceData)
    {
        return IsHeroInParty(heroInstanceData.InstanceId);
    }
}

