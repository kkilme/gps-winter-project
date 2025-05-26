using System.Collections;
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
        HeroParty.Heroes.Clear();
        HeroParty.HeroesDict.Clear();
        foreach (int heroInstanceId in HeroParty.HeroIds)
        {
            if (HeroStorage.SavedHeroDatas.ContainsKey(heroInstanceId))
            {
                var data = HeroStorage.SavedHeroDatas[heroInstanceId];
                HeroStat stat = HeroStorage.GetHeroStat(heroInstanceId);
                stat.ClearBuffAndDebuffStats();
                if (stat != null)
                {
                    GameObject go = Managers.ResourceMng.Instantiate(GlobalValues.HERO_PREFAB_PATH_PREFIX + data.ClassName, _heroRoot);

                    Hero hero = go.GetOrAddComponent<Hero>();

                    // 저장된 스탯 적용
                    hero.SetData(data.HeroDataId, stat);

                    // 저장된 무기 장착
                    int weaponDataId = HeroStorage.GetEquippedWeapon(heroInstanceId);
                    if (weaponDataId != -1)
                        hero.EquipWeapon(weaponDataId);

                    // 저장된 방어구 장착
                    var equippedArmors = HeroStorage.GetEquippedArmors(heroInstanceId);
                    if (equippedArmors != null)
                    {
                        foreach (var armors in equippedArmors)
                        {
                            if(armors.Value == -1) continue;
                            Armor armor = new Armor(armors.Value);
                            hero.EquipArmor(armor);
                        }
                    }
                    
                    HeroParty.AddRuntimeHero(heroInstanceId, hero);
                }
            }
        }
    }

    /// <summary>
    /// 보유한 모든 영웅의 데이터 반환.
    /// </summary>
    public List<SavedHeroData> GetSavedHeroDatas()
    {
        List<SavedHeroData> savedHeroDatas = new List<SavedHeroData>();
        foreach (var data in HeroStorage.SavedHeroDatas)
        {
            savedHeroDatas.Add(data.Value);
        }
        return savedHeroDatas;
    }
}

