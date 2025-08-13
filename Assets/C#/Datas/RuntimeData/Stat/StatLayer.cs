using System;

[Serializable]
public struct StatLayer
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
        a.MaxHp += b.MaxHp;
        a.BaseDamage += b.BaseDamage;
        a.PhysicalDefense += b.PhysicalDefense;
        a.MagicDefense += b.MagicDefense;
        a.Strength += b.Strength;
        a.Vitality += b.Vitality;
        a.Intelligence += b.Intelligence;
        a.Dexterity += b.Dexterity;
        return a;
    }

    public static StatLayer operator -(StatLayer a, StatLayer b)
    {
        a.MaxHp -= b.MaxHp;
        a.BaseDamage -= b.BaseDamage;
        a.PhysicalDefense -= b.PhysicalDefense;
        a.MagicDefense -= b.MagicDefense;
        a.Strength -= b.Strength;
        a.Vitality -= b.Vitality;
        a.Intelligence -= b.Intelligence;
        a.Dexterity -= b.Dexterity;
        return a;
    }
}