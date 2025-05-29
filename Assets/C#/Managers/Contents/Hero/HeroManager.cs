using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        HeroParty.RuntimeHeroes.Clear();
        HeroParty.RuntimeHeroesDict.Clear();
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

                    // 저장된 방어구 장착
                    var equippedArmors = HeroStorage.GetEquippedArmors(heroInstanceId);
                    if (equippedArmors != null)
                    {
                        foreach (var armors in equippedArmors)
                        {
                            if(armors.Value == null) continue;
                            Armor armor = new Armor(armors.Value.ItemDataId);
                            hero.EquipArmor(armor);
                        }
                    }
                    
                    HeroParty.AddRuntimeHero(heroInstanceId, hero);
                }
            }
        }
    }
}

