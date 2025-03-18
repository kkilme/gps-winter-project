using System;
using UnityEngine;

public class CreatureStat : MonoBehaviour
{
    private string _name;
    private int _hp;
    private int _maxHp;
    private int _baseDamage;
    private int _physicalDefense;
    private int _magicDefense;
    private int _strength;
    private int _vitality;
    private int _intelligence;
    private int _dexterity;

    public string Name { get => _name; }
    public int Hp { get => _hp; set { _hp = value; StatChangeAction?.Invoke(this); } }
    public int MaxHp { get => _maxHp; set { _maxHp = value; StatChangeAction?.Invoke(this); } }
    public int BaseDamage { get => _baseDamage; set { _baseDamage = value; StatChangeAction?.Invoke(this); } }
    public int PhysicalDefense { get => _physicalDefense; set { _physicalDefense = value; StatChangeAction?.Invoke(this); } }
    public int MagicDefense { get => _magicDefense; set { _magicDefense = value; StatChangeAction?.Invoke(this); } }
    

    public int Strength { get => _strength; set { _strength = value; StatChangeAction?.Invoke(this); } }
    public int Vitality { get => _vitality; set { _vitality = value; StatChangeAction?.Invoke(this); } }
    public int Intelligence { get => _intelligence; set { _intelligence = value; StatChangeAction?.Invoke(this); } }
    public int Dexterity { get => _dexterity; set { _dexterity = value; StatChangeAction?.Invoke(this); } }

    public Action<CreatureStat> StatChangeAction;

    public virtual void SetStat(Data.CreatureData creatureData)
    {
        StatChangeAction = null;
        _name = creatureData.Name;
        _hp = creatureData.Hp;
        _maxHp = creatureData.Hp;
        _baseDamage = creatureData.BaseDamage;
        _physicalDefense = creatureData.PhysicalDefense;
        _magicDefense = creatureData.MagicDefense;
        _strength = creatureData.Strength;
        _vitality = creatureData.Vitality;
        _intelligence = creatureData.Intelligence;
        _dexterity = creatureData.Dexterity;
    }

    public int NameToStat(StatName stat)
    {
        return stat switch
        {
            StatName.Strength => Strength,
            StatName.Vitality => Vitality,
            StatName.Intelligence => Intelligence,
            StatName.Dexterity => Dexterity,
            _ => -1,
        };
    }
    #region Event

    public void TakeDamage(int damage)
    {
        Hp = Mathf.Clamp(Hp - damage, 0, MaxHp);
    }

    public void TakeHeal(int amount)
    {
        Hp = Mathf.Clamp(Hp + amount, 0, MaxHp);
    }

    #endregion
    #region Equipment
    public void AttachEquipment(Data.EquipmentData equipmentData)
    {
        Hp += equipmentData.Hp;
        MaxHp += equipmentData.Hp;
        BaseDamage += equipmentData.Attack;
        PhysicalDefense += equipmentData.PhysicalDefense;
        MagicDefense += equipmentData.MagicDefense;
        Strength += equipmentData.Strength;
        Vitality += equipmentData.Vitality;
        Intelligence += equipmentData.Intelligence;
        Dexterity += equipmentData.Dexterity;
    }

    public void DetachEquipment(Data.EquipmentData equipmentData)
    {
        Hp -= equipmentData.Hp;
        MaxHp -= equipmentData.Hp;
        BaseDamage -= equipmentData.Attack;
        PhysicalDefense -= equipmentData.PhysicalDefense;
        MagicDefense -= equipmentData.MagicDefense;
        Strength -= equipmentData.Strength;
        Vitality -= equipmentData.Vitality;
        Intelligence -= equipmentData.Intelligence;
        Dexterity -= equipmentData.Dexterity;
    }
    #endregion
}
