using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

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
    /// 영웅에게 장비 장착 및 스탯 적용.
    /// </summary>
    public void EquipEquipment(int heroInstanceId, EquipmentType type, int equipmentDataId)
    {
        if (SavedHeroDatas.TryGetValue(heroInstanceId, out SavedHeroData savedHeroData))
        {
            switch (type)
            {
                case EquipmentType.Weapon:
                    EquipWeapon(heroInstanceId, equipmentDataId);
                    break;
                case EquipmentType.Helmet:
                    EquipArmor(heroInstanceId, ArmorType.Helmet, equipmentDataId);
                    break;
                case EquipmentType.Body:
                    EquipArmor(heroInstanceId, ArmorType.Body, equipmentDataId);
                    break;
                case EquipmentType.Cloak:
                    EquipArmor(heroInstanceId, ArmorType.Cloak, equipmentDataId);
                    break;
            }
        }
    }

    public void EquipEquipment(int heroInstanceId, UI_InventorySlot slot)
    {
        if(slot.IsEmpty || slot.ItemData is not EquipmentData equipmentData)
        {
            return;
        }

        EquipEquipment(heroInstanceId, equipmentData.EquipmentType, equipmentData.DataId);
    }

    /// <summary>
    /// 영웅이 장착중인 장비 해제
    /// </summary>
    public void UnEquipEquipment(int heroInstanceId, EquipmentType type)
    {
        if (SavedHeroDatas.TryGetValue(heroInstanceId, out SavedHeroData savedHeroData))
        {
            switch (type)
            {
                case EquipmentType.Weapon:
                    UnEquipWeapon(heroInstanceId);
                    break;
                case EquipmentType.Helmet:
                    UnEquipArmor(heroInstanceId, ArmorType.Helmet);
                    break;
                case EquipmentType.Body:
                    UnEquipArmor(heroInstanceId, ArmorType.Body);
                    break;
                case EquipmentType.Cloak:
                    UnEquipArmor(heroInstanceId, ArmorType.Cloak);
                    break;
            }
        }
    }

    public int GetEquippedEquipment(int heroInstanceId, EquipmentType type)
    {
        if (SavedHeroDatas.TryGetValue(heroInstanceId, out var data))
        {
            switch (type)
            {
                case EquipmentType.Weapon:
                    return data.Weapon;
                case EquipmentType.Helmet:
                    return data.Armors != null && data.Armors.TryGetValue(ArmorType.Helmet, out int helmet) ? helmet : -1;
                case EquipmentType.Body:
                    return data.Armors != null && data.Armors.TryGetValue(ArmorType.Body, out int body) ? body : -1;
                case EquipmentType.Cloak:
                    return data.Armors != null && data.Armors.TryGetValue(ArmorType.Cloak, out int cloak) ? cloak : -1;
            }
        }
        return -1;
    }


    /// <summary>
    /// 영웅에게 무기 장착 및 스탯 적용.
    /// </summary>
    public void EquipWeapon(int heroInstanceId, int weaponDataId)
    {
        if (SavedHeroDatas.TryGetValue(heroInstanceId, out SavedHeroData savedHeroData))
        {
            UnEquipWeapon(heroInstanceId); // 기존 무기 해제

            savedHeroData.Weapon = weaponDataId;
            savedHeroData.Stat.AddEquipmentStat(Managers.DataMng.WeaponDataDict[weaponDataId]);
            savedHeroData.OnStatChanged?.Invoke(savedHeroData);
        }
    }

    /// <summary>
    /// 영웅이 착용중인 무기 해제.
    /// </summary>
    /// <param name="heroInstanceId"></param>
    public void UnEquipWeapon(int heroInstanceId)
    {
        if (SavedHeroDatas.TryGetValue(heroInstanceId, out SavedHeroData savedHeroData) && savedHeroData.Weapon != -1)
        {
            int weaponDataId = savedHeroData.Weapon;

            savedHeroData.Stat.RemoveEquipmentStat(Managers.DataMng.WeaponDataDict[weaponDataId]);
            savedHeroData.OnStatChanged?.Invoke(savedHeroData);
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
        if (SavedHeroDatas.TryGetValue(heroInstanceId, out SavedHeroData savedHeroData))
        {
            UnEquipArmor(heroInstanceId, type); // 기존 방어구 해제

            savedHeroData.Armors[type] = armorDataId;
            savedHeroData.Stat.AddEquipmentStat(Managers.DataMng.ArmorDataDict[armorDataId]);
            savedHeroData.OnStatChanged?.Invoke(savedHeroData);
        }
    }

    /// <summary>
    /// 영웅이 착용중인 방어구 해제.
    /// </summary>
    public void UnEquipArmor(int heroInstanceId, ArmorType type)
    {
        if (SavedHeroDatas.TryGetValue(heroInstanceId, out SavedHeroData savedHeroData) && savedHeroData.Armors[type] != -1)
        {
            int armorDataId = savedHeroData.Armors[type];
            savedHeroData.Stat.RemoveEquipmentStat(Managers.DataMng.ArmorDataDict[armorDataId]);
            savedHeroData.OnStatChanged?.Invoke(savedHeroData);
            savedHeroData.Armors[type] = -1;
        }
    }

    public Dictionary<ArmorType, int> GetEquippedArmors(int heroInstanceId)
    {
        return SavedHeroDatas.TryGetValue(heroInstanceId, out var data) ? data.Armors : null;
    }
}
