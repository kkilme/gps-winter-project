using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class ItemData
{
    public int DataId;
    public string Name;
    public string ClassName; // Item을 상속받는 클래스명
    public ItemType ItemType;
    public string ImagePath;
    public int MaxStack; // 인벤토리에서 최대로 스택될 수 있는 양
}

[Serializable]
public class ConsumableItemData : ItemData
{
    public string Description;
}

[Serializable]
public class EquipmentData : ItemData
{
    public EquipmentType EquipmentType;
    public int Hp;
    public int Attack;
    public int PhysicalDefense;
    public int MagicDefense;
    public int Dexterity;
    public int Strength;
    public int Vitality;
    public int Intelligence;
}

[Serializable]
public class WeaponData : EquipmentData
{
    public int LeftIndex;
    public int RightIndex;
    public WeaponType WeaponType;
    public List<int> Skills;
}

[Serializable]
public class ArmorData : EquipmentData
{
    public int ArmorIndex;
    public ArmorType ArmorType;
}

[Serializable]
public class ConsumableItemDataLoader : ILoader<int, ConsumableItemData>
{
    public List<ConsumableItemData> consumableItems = new List<ConsumableItemData>();
    public Dictionary<int, ConsumableItemData> MakeDict()
    {
        var dic = new Dictionary<int, ConsumableItemData>();
        foreach (var item in consumableItems)
        {
            item.ClassName ??= item.Name;
            item.ItemType = ItemType.Consumable;
            item.MaxStack = item.MaxStack == 0 ? 99 : item.MaxStack;
            item.ImagePath ??= "Default_Consumable";
            dic.Add(item.DataId, item);
        }
        return dic;
    }
}

[Serializable]
public class WeaponDataLoader : ILoader<int, WeaponData>
{
    public List<WeaponData> weapons = new List<WeaponData>();

    public Dictionary<int, WeaponData> MakeDict()
    {
        var dic = new Dictionary<int, WeaponData>();
        foreach (var weapon in weapons)
        {
            weapon.ClassName ??= weapon.Name;
            weapon.ItemType = ItemType.Weapon;
            weapon.EquipmentType = EquipmentType.Weapon;
            weapon.MaxStack = 1;
            weapon.ImagePath ??= "Default_Weapon";
            dic.Add(weapon.DataId, weapon);
        }

        return dic;
    }
}

[Serializable]
public class ArmorDataLoader : ILoader<int, ArmorData>
{
    public List<ArmorData> armors = new List<ArmorData>();

    public Dictionary<int, ArmorData> MakeDict()
    {
        var dic = new Dictionary<int, ArmorData>();
        foreach (var armor in armors) 
        {
            armor.ClassName ??= armor.Name;
            armor.ItemType = ItemType.Armor;
            armor.EquipmentType = armor.ArmorType switch
            {
                ArmorType.Helmet => EquipmentType.Helmet,
                ArmorType.Body => EquipmentType.Body,
                ArmorType.Cloak => EquipmentType.Cloak,
                _ => EquipmentType.None
            };
            armor.MaxStack = 1;
            armor.ImagePath ??= "Default_Armor";
            dic.Add(armor.DataId, armor);
        }

        return dic;
    }
}
