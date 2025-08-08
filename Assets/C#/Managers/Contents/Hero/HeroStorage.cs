using System.Collections.Generic;

/// <summary>
/// 플레이어가 소유한 모든 영웅의 데이터를 저장 및 관리
/// </summary>
public class HeroStorage
{
    private int _nextHeroInstanceId = 0;
    public Dictionary<int, HeroInstanceData> _ownedHeroes { get; private set; } = new(); // key: heroInstanceId, value: HeroInstanceData

    private HeroParty _heroParty => Managers.HeroMng.HeroParty;

    /// <summary>
    /// 새로운 영웅 추가
    /// </summary>
    public int AddHero(int heroDataId)
    {
        int heroInstanceId = _nextHeroInstanceId++;

        _ownedHeroes[heroInstanceId] = new HeroInstanceData(heroDataId, heroInstanceId);

        return heroInstanceId;
    }

    /// <summary>
    /// 특정 영웅 제거
    /// </summary>
    public void RemoveHero(int heroInstanceId)
    {
        _ownedHeroes.Remove(heroInstanceId);
    }

    public HeroInstanceData GetHeroInstanceData(int heroInstanceId)
    {
        return _ownedHeroes.TryGetValue(heroInstanceId, out var data) ? data : null;
    }

    public HeroStat GetHeroStat(int heroInstanceId)
    {
        return GetHeroInstanceData(heroInstanceId)?.Stat;
    }

    public List<int> GetAllOwnedHeroInstanceIds()
    {
        return new List<int>(_ownedHeroes.Keys);
    }

    public List<HeroInstanceData> GetAllOwnedHeroDatas()
    {
        return new List<HeroInstanceData>(_ownedHeroes.Values);
    }


    /// <summary>
    /// 영웅에게 장비 장착 및 스탯 적용.
    /// </summary>
    public void EquipEquipment(int heroInstanceId, EquipmentType type, int equipmentInstanceId)
    {
        if (_ownedHeroes.TryGetValue(heroInstanceId, out HeroInstanceData savedHeroData)
            && Managers.InvMng.IsValidEquipment(equipmentInstanceId, type))
        {
            EquipmentInstanceData equipmentInstance = Managers.InvMng.ItemDict[equipmentInstanceId] as EquipmentInstanceData;
            if (equipmentInstance.IsEquipped)
            {
                UnEquipEquipment(equipmentInstance.EquippedHeroId, type); // 이미 다른 영웅이 장착중인 경우 해당 영웅에서 해제
            }

            switch (type)
            {
                case EquipmentType.Weapon:
                    EquipWeapon(heroInstanceId, equipmentInstanceId);
                    break;
                case EquipmentType.Helmet:
                    EquipArmor(heroInstanceId, ArmorType.Helmet, equipmentInstanceId);
                    break;
                case EquipmentType.Body:
                    EquipArmor(heroInstanceId, ArmorType.Body, equipmentInstanceId);
                    break;
                case EquipmentType.Cloak:
                    EquipArmor(heroInstanceId, ArmorType.Cloak, equipmentInstanceId);
                    break;
            }

            equipmentInstance.EquippedHeroId = heroInstanceId;
            savedHeroData.OnEquipmentChanged?.Invoke(savedHeroData);
        }
    }

    /// <summary>
    /// 영웅이 장착중인 장비 해제
    /// </summary>
    public void UnEquipEquipment(int heroInstanceId, EquipmentType type)
    {
        if (_ownedHeroes.TryGetValue(heroInstanceId, out HeroInstanceData savedHeroData))
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

            savedHeroData.OnEquipmentChanged?.Invoke(savedHeroData);
        }
    }

    /// <summary>
    /// 영웅에게 방어구 장착 및 스탯 적용.
    /// </summary>
    private void EquipArmor(int heroInstanceId, ArmorType type, int armorInstanceId)
    {
        if (_ownedHeroes.TryGetValue(heroInstanceId, out HeroInstanceData savedHeroData))
        {
            UnEquipArmor(heroInstanceId, type); // 기존 방어구 해제

            EquipmentInstanceData armorInstance = Managers.InvMng.ItemDict[armorInstanceId] as EquipmentInstanceData;
            savedHeroData.Armors[type] = armorInstance;
            savedHeroData.Stat.AddEquipmentStat(armorInstance.EquipmentData);

            // 런타임 영웅에 있는 영웅이라면 장비 장착
            if (_heroParty.RuntimeHeroesDict.TryGetValue(heroInstanceId, out Hero hero))
            {
                hero.EquipArmor(armorInstance.ItemDataId);
            }
        }
    }

    /// <summary>
    /// 영웅이 착용중인 방어구 해제.
    /// </summary>
    private void UnEquipArmor(int heroInstanceId, ArmorType type)
    {
        if (_ownedHeroes.TryGetValue(heroInstanceId, out HeroInstanceData savedHeroData) && savedHeroData.Armors[type] != null)
        {
            EquipmentInstanceData armorInstance = savedHeroData.Armors[type];
            EquipmentData armorData = armorInstance.EquipmentData;

            savedHeroData.Stat.RemoveEquipmentStat(armorData);
            savedHeroData.Armors[type] = null;
            armorInstance.EquippedHeroId = -1;

            // 런타임 영웅에 있는 영웅이라면 장비 해제
            if (_heroParty.RuntimeHeroesDict.TryGetValue(heroInstanceId, out Hero hero))
            {
                hero.UnEquipArmor(type);
            }
        }
    }

    /// <summary>
    /// 영웅이 장착중인 모든 방어구를 반환.
    /// </summary>
    public Dictionary<ArmorType, EquipmentInstanceData> GetEquippedArmors(int heroInstanceId)
    {
        return _ownedHeroes.TryGetValue(heroInstanceId, out var data) ? data.Armors : null;
    }

    /// <summary>
    /// 영웅이 장착중인 특정 타입의 장비를 반환. 해당 영웅이 장착중이지 않거나 해당 타입의 장비가 없는 경우 null 반환.
    /// </summary>
    public EquipmentInstanceData GetEquippedEquipment(int heroInstanceId, EquipmentType type)
    {
        if (_ownedHeroes.TryGetValue(heroInstanceId, out var data))
        {
            switch (type)
            {
                case EquipmentType.Weapon:
                    return data.Weapon;
                case EquipmentType.Helmet:
                    return data.Armors != null && data.Armors.TryGetValue(ArmorType.Helmet, out EquipmentInstanceData helmet) ? helmet : null;
                case EquipmentType.Body:
                    return data.Armors != null && data.Armors.TryGetValue(ArmorType.Body, out EquipmentInstanceData body) ? body : null;
                case EquipmentType.Cloak:
                    return data.Armors != null && data.Armors.TryGetValue(ArmorType.Cloak, out EquipmentInstanceData cloak) ? cloak : null;
            }
        }
        return null;
    }

    /// <summary>
    /// 영웅에게 무기 장착 및 스탯 적용.
    /// </summary>
    private void EquipWeapon(int heroInstanceId, int weaponInstanceId)
    {
        if (_ownedHeroes.TryGetValue(heroInstanceId, out HeroInstanceData savedHeroData)
            && Managers.InvMng.IsValidEquipment(weaponInstanceId, EquipmentType.Weapon))
        {
            UnEquipWeapon(heroInstanceId); // 기존 무기 해제

            EquipmentInstanceData weaponInstance = Managers.InvMng.ItemDict[weaponInstanceId] as EquipmentInstanceData;
            savedHeroData.Weapon = weaponInstance;
            savedHeroData.Stat.AddEquipmentStat(weaponInstance.EquipmentData);

            // 런타임 영웅에 있는 영웅이라면 무기 장착
            if (_heroParty.RuntimeHeroesDict.TryGetValue(heroInstanceId, out Hero hero))
            {
                hero.EquipWeapon(weaponInstance.ItemDataId);
            }
        }
    }

    /// <summary>
    /// 영웅이 착용중인 무기 해제.
    /// </summary>
    private void UnEquipWeapon(int heroInstanceId)
    {
        if (_ownedHeroes.TryGetValue(heroInstanceId, out HeroInstanceData savedHeroData) && savedHeroData.Weapon != null)
        {
            EquipmentInstanceData weaponInstance = savedHeroData.Weapon;
            WeaponData weaponData = weaponInstance.EquipmentData as WeaponData;

            savedHeroData.Stat.RemoveEquipmentStat(weaponData);
            savedHeroData.Weapon = null;
            weaponInstance.EquippedHeroId = -1;

            // 런타임 영웅에 있는 영웅이라면 무기 해제
            if (_heroParty.RuntimeHeroesDict.TryGetValue(heroInstanceId, out Hero hero))
            {
                hero.EquipWeapon(GlobalValues.HERO_HANDWEAPON_ID);
            }
        }
    }

    public EquipmentInstanceData GetEquippedWeapon(int heroInstanceId)
    {
        return _ownedHeroes.TryGetValue(heroInstanceId, out var data) ? data.Weapon : null;
    }

}
