using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class CreatureData
    {
        public int DataId;
        public string Name;
        public int Hp;
        public int BaseDamage;
        public int PhysicalDefense;
        public int MagicDefense;
        public int Strength;
        public int Vitality;
        public int Intelligence;
        public int Dexterity;
    }

    [Serializable]
    public class MonsterData : CreatureData
    {
        public List<int> Actions = new List<int>();
        public int MinGold;
        public int MaxGold;
        public List<ItemLootData> LootTable = new List<ItemLootData>();
    }

    [Serializable]
    public class HeroDataLoader : ILoader<int, CreatureData>
    {
        public List<CreatureData> heroes = new List<CreatureData>();

        public Dictionary<int, CreatureData> MakeDict()
        {
            Dictionary<int, CreatureData> dic = new Dictionary<int, CreatureData>();
            foreach (CreatureData hero in heroes)
            {
                dic.Add(hero.DataId, hero);
            }

            return dic;
        }
    }

    [Serializable]
    public class MonsterDataLoader : ILoader<int, MonsterData>
    {
        public List<MonsterData> monsters = new List<MonsterData>();

        public Dictionary<int, MonsterData> MakeDict()
        {
            Dictionary<int, MonsterData> dic = new Dictionary<int, MonsterData>();
            foreach (MonsterData monster in monsters)
                dic.Add(monster.DataId, monster);

            return dic;
        }
    }
}