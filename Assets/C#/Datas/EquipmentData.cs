using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class EquipmentData
    {
        public int DataId;
        public string Name;
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
    }

    [Serializable]
    public class WeaponDataLoader : ILoader<int, WeaponData>
    {
        public List<WeaponData> weapons = new List<WeaponData>();

        public Dictionary<int, WeaponData> MakeDict()
        {
            var dic = new Dictionary<int, WeaponData>();
            foreach (var weapon in weapons)
                dic.Add(weapon.DataId, weapon);

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
                dic.Add(armor.DataId, armor);

            return dic;
        }
    }
}