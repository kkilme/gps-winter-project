using System;
using UnityEngine;

public class CreatureStat
{
    public virtual StatLayer FinalStat => BaseStat + BuffStat + DebuffStat;

    public StatLayer BaseStat { get; protected set; }
    public StatLayer BuffStat { get; protected set; } // TODO: 버프와 디버프는 추후 구현
    public StatLayer DebuffStat { get; protected set; }


    public Action<CreatureStat> StatChangeAction;


    private int _currentHp;
    public int Hp
    {
        get => _currentHp;
        set
        {
            _currentHp = Mathf.Clamp(value, 0, FinalStat.MaxHp);
            StatChangeAction?.Invoke(this);
        }
    }

    public int MaxHp => FinalStat.MaxHp;
    public int BaseDamage => FinalStat.BaseDamage;
    public int PhysicalDefense => FinalStat.PhysicalDefense;
    public int MagicDefense => FinalStat.MagicDefense;
    public int Strength => FinalStat.Strength;
    public int Vitality => FinalStat.Vitality;
    public int Intelligence => FinalStat.Intelligence;
    public int Dexterity => FinalStat.Dexterity;

    public int NameToStat(StatName stat)
    {
        return stat switch
        {
            StatName.BaseDamage => BaseDamage,
            StatName.MaxHp => MaxHp,
            StatName.PhysicalDefense => PhysicalDefense,
            StatName.MagicDefense => MagicDefense,
            StatName.Strength => Strength,
            StatName.Vitality => Vitality,
            StatName.Intelligence => Intelligence,
            StatName.Dexterity => Dexterity,
            _ => -1,
        };
    }

    public CreatureStat(CreatureData creatureData)
    {

        BaseStat = new StatLayer
        {
            MaxHp = creatureData.Hp,
            BaseDamage = creatureData.BaseDamage,
            PhysicalDefense = creatureData.PhysicalDefense,
            MagicDefense = creatureData.MagicDefense,
            Strength = creatureData.Strength,
            Vitality = creatureData.Vitality,
            Intelligence = creatureData.Intelligence,
            Dexterity = creatureData.Dexterity,
        };

        _currentHp = creatureData.Hp;

        BuffStat = new StatLayer();
        DebuffStat = new StatLayer();
    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;
    }

    public void TakeHeal(int amount)
    {
        Hp += amount;
    }
}
