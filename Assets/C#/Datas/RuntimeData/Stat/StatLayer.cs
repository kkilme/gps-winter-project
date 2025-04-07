using System;

[Serializable]
public class StatLayer
{
    public int MaxHp;
    public int BaseDamage;
    public int PhysicalDefense;
    public int MagicDefense;
    public int Strength;
    public int Vitality;
    public int Intelligence;
    public int Dexterity;

    public static StatLayer operator +(StatLayer a, StatLayer b)
    {
        return new StatLayer
        {
            MaxHp = a.MaxHp + b.MaxHp,
            BaseDamage = a.BaseDamage + b.BaseDamage,
            PhysicalDefense = a.PhysicalDefense + b.PhysicalDefense,
            MagicDefense = a.MagicDefense + b.MagicDefense,
            Strength = a.Strength + b.Strength,
            Vitality = a.Vitality + b.Vitality,
            Intelligence = a.Intelligence + b.Intelligence,
            Dexterity = a.Dexterity + b.Dexterity,
        };
    }

    public static StatLayer operator -(StatLayer a, StatLayer b)
    {
        return new StatLayer
        {
            MaxHp = a.MaxHp - b.MaxHp,
            BaseDamage = a.BaseDamage - b.BaseDamage,
            PhysicalDefense = a.PhysicalDefense - b.PhysicalDefense,
            MagicDefense = a.MagicDefense - b.MagicDefense,
            Strength = a.Strength - b.Strength,
            Vitality = a.Vitality - b.Vitality,
            Intelligence = a.Intelligence - b.Intelligence,
            Dexterity = a.Dexterity - b.Dexterity,
        };
    }
}