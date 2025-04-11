using System.Collections;
using UnityEngine;
using System.Collections.Generic;

// 플레이어가 소유한 모든 영웅의 데이터를 저장 및 관리
public class HeroStorage
{
    private int _nextHeroInstanceId = 0;
    public Dictionary<int, SavedHeroData> SavedHeroDatas { get; private set; } = new(); // key: heroInstanceId, value: SavedHeroData

    /// <summary>
    /// 새로운 영웅 추가
    /// </summary>
    public int AddHero(int heroDataId)
    {
        int heroInstanceId = _nextHeroInstanceId++;

        SavedHeroDatas[heroInstanceId] = new SavedHeroData(heroDataId, heroInstanceId);

        return heroInstanceId;
    }

    public void RemoveHero(int heroInstanceId)
    {
        SavedHeroDatas.Remove(heroInstanceId);
    }

    public HeroStat GetHeroStat(int heroInstanceId)
    {
        return SavedHeroDatas.TryGetValue(heroInstanceId, out var data) ? data.Stat : null;
    }

    public void SaveWeapon(int heroInstanceId, int weaponDataId)
    {
        if (SavedHeroDatas.ContainsKey(heroInstanceId))
            SavedHeroDatas[heroInstanceId].Weapon = weaponDataId;
    }

    public int GetEquippedWeapon(int heroInstanceId)
    {
        return SavedHeroDatas.TryGetValue(heroInstanceId, out var data) ? data.Weapon : -1;
    }

    public void SaveArmor(int heroInstanceId, ArmorType type, int armorDataId)
    {
        if(SavedHeroDatas.ContainsKey(heroInstanceId))
            SavedHeroDatas[heroInstanceId].Armors[type] = armorDataId;
    }

    public Dictionary<ArmorType, int> GetEquippedArmors(int heroInstanceId)
    {
        return SavedHeroDatas.TryGetValue(heroInstanceId, out var data) ? data.Armors : null;
    }
}
