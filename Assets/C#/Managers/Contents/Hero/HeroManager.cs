using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroManager
{
    public HeroParty HeroParty { get; protected set; } = new HeroParty();
    private HeroStorage _heroStorage => Managers.StorageMng.HeroStorage;
    private Transform _heroRoot => GlobalUtility.FindOrCreateTransform("@Heroes");

    /// <summary>
    /// 현재 파티에 편성된 영웅들을 게임오브젝트로 스폰. 오브젝트의 Transform 설정은 별도로 해주어야 함에 주의.
    /// </summary>
    public void SpawnHeroParty()
    {
        HeroParty.Heroes.Clear();
        HeroParty.HeroesDict.Clear();
        foreach (int heroInstanceId in HeroParty.HeroIds)
        {
            if (_heroStorage.SavedHeroDatas.ContainsKey(heroInstanceId))
            {
                var data = _heroStorage.SavedHeroDatas[heroInstanceId];
                HeroStat stat = _heroStorage.GetHeroStat(heroInstanceId);
                stat.ClearBonusStats(); // 영웅 스폰 시, BaseStat을 제외한 스탯 초기화
                if (stat != null)
                {
                    GameObject go = Managers.ResourceMng.Instantiate(GlobalValues.HERO_PREFAB_PATH_PREFIX + data.ClassName, _heroRoot);

                    Hero hero = go.GetOrAddComponent<Hero>();

                    // 저장된 스탯 적용
                    hero.SetData(data.HeroDataId, stat);

                    // 저장된 무기 장착
                    int weaponDataId = _heroStorage.GetEquippedWeapon(heroInstanceId);
                    if (weaponDataId != -1)
                        hero.EquipWeapon(weaponDataId);

                    // 저장된 방어구 장착
                    var equippedArmors = _heroStorage.GetEquippedArmors(heroInstanceId);
                    if (equippedArmors != null)
                    {
                        foreach (var armors in equippedArmors)
                        {
                            Armor armor = new Armor(armors.Value);
                            hero.EquipArmor(armor);
                        }
                    }
                    
                    HeroParty.AddRuntimeHero(heroInstanceId, hero);
                }
            }
        }
    }

    // 테스트용: Knight 2명, Wizard 2명 소환
    public void AddHeroesOnTest()
    {
        for (int i = 0; i < 2; i++)
        {
            _heroStorage.AddHero(GlobalValues.HERO_KNIGHT_ID);
        }

        for (int i = 0; i < 2; i++)
        {
            _heroStorage.AddHero(GlobalValues.HERO_WIZARD_ID);
        }
        foreach (var key in _heroStorage.SavedHeroDatas.Keys)
        {
            HeroParty.AddHero(key);
        }
    }
}

