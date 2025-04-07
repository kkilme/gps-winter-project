using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


public class HeroData: CreatureData
{
    public int StartWeapon; // 시작 무기의 DataId
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
public class HeroDataLoader : ILoader<int, HeroData>
{
    public List<HeroData> heroes = new List<HeroData>();

    public Dictionary<int, HeroData> MakeDict()
    {
        Dictionary<int, HeroData> dic = new Dictionary<int, HeroData>();
        foreach (HeroData hero in heroes)
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