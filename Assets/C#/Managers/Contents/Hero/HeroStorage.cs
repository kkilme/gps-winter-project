using System.Collections;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 플레이어가 소유한 모든 영웅의 데이터를 저장 및 관리
/// </summary>
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

    /// <summary>
    /// 영웅에게 무기 장착 및 스탯 적용.
    /// </summary>
    public void EquipWeapon(int heroInstanceId, int weaponDataId)
    {
        if (SavedHeroDatas.ContainsKey(heroInstanceId))
        {
            UnEquipWeapon(heroInstanceId); // 기존 무기 해제
            SavedHeroDatas[heroInstanceId].Weapon = weaponDataId;
            SavedHeroDatas[heroInstanceId].Stat.AddEquipmentStat(Managers.DataMng.WeaponDataDict[weaponDataId]);
        }
    }

    /// <summary>
    /// 영웅이 착용중인 무기 해제.
    /// </summary>
    /// <param name="heroInstanceId"></param>
    public void UnEquipWeapon(int heroInstanceId)
    {
        if (SavedHeroDatas.ContainsKey(heroInstanceId) && SavedHeroDatas[heroInstanceId].Weapon != -1)
        {
            SavedHeroData savedHeroData = SavedHeroDatas[heroInstanceId];
            int weaponDataId = savedHeroData.Weapon;
            savedHeroData.Stat.RemoveEquipmentStat(Managers.DataMng.WeaponDataDict[weaponDataId]);
            savedHeroData.Weapon = -1;
        }
    }

    public int GetEquippedWeapon(int heroInstanceId)
    {
        return SavedHeroDatas.TryGetValue(heroInstanceId, out var data) ? data.Weapon : -1;
    }

    /// <summary>
    /// 영웅에게 방어구 장착 및 스탯 적용.
    /// </summary>
    public void EquipArmor(int heroInstanceId, ArmorType type, int armorDataId)
    {
        if (SavedHeroDatas.ContainsKey(heroInstanceId))
        {
            UnEquipArmor(heroInstanceId, type); // 기존 방어구 해제
            SavedHeroDatas[heroInstanceId].Armors[type] = armorDataId;
            SavedHeroDatas[heroInstanceId].Stat.AddEquipmentStat(Managers.DataMng.ArmorDataDict[armorDataId]);
        }
    }

    /// <summary>
    /// 영웅이 착용중인 방어구 해제.
    /// </summary>
    public void UnEquipArmor(int heroInstanceId, ArmorType type)
    {
        if (SavedHeroDatas.ContainsKey(heroInstanceId) && SavedHeroDatas[heroInstanceId].Armors[type] != -1)
        {
            SavedHeroData savedHeroData = SavedHeroDatas[heroInstanceId];
            int armorDataId = SavedHeroDatas[heroInstanceId].Armors[type];
            savedHeroData.Stat.RemoveEquipmentStat(Managers.DataMng.ArmorDataDict[armorDataId]);
            savedHeroData.Armors[type] = -1;
        }
    }

    public Dictionary<ArmorType, int> GetEquippedArmors(int heroInstanceId)
    {
        return SavedHeroDatas.TryGetValue(heroInstanceId, out var data) ? data.Armors : null;
    }
}
