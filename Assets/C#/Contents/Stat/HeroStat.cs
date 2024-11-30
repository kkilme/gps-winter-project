using UnityEngine;
using System;
using Data;

public class HeroStat : CreatureStat
{
    private int _strength;
    private int _vitality;
    private int _intelligence;
    private int _dexterity;
    
    public int Strength { get => _strength; set { _strength = value; StatChangeAction?.Invoke(this); } }
    public int Vitality { get => _vitality; set { _vitality = value; StatChangeAction?.Invoke(this); } }
    public int Intelligence { get => _intelligence; set { _intelligence = value; StatChangeAction?.Invoke(this); } }
    public int Dexterity { get => _dexterity; set { _dexterity = value; StatChangeAction?.Invoke(this); } }

    public override void SetStat(Data.CreatureData creatureData)
    {
        base.SetStat(creatureData);

        Data.HeroData heroData = (HeroData)creatureData;
        _strength = heroData.Strength;
        _vitality = heroData.Vitality;
        _intelligence = heroData.Intelligence;
        _dexterity = heroData.Dexterity;
    }

    public int GetStatByDefine(Define.Stat stat)
    {
        switch (stat)
        {
            case Define.Stat.Strength:
                return Strength;
            case Define.Stat.Vitality:
                return Vitality;
            case Define.Stat.Intelligence:
                return Intelligence;
            case Define.Stat.Dexterity:
                return Dexterity;
        }

        return -1;
    }
    
    #region Equipment
    public void AttachEquipment(Data.EquipmentData equipmentData)
    {
        Hp += equipmentData.Hp;
        MaxHp += equipmentData.Hp;
        Attack += equipmentData.Attack;
        Defense += equipmentData.Defense;
        Strength += equipmentData.Strength;
        Vitality += equipmentData.Vitality;
        Intelligence += equipmentData.Intelligence;
        Dexterity += equipmentData.Dexterity;
    }

    public void DetachEquipment(Data.EquipmentData equipmentData)
    {
        Hp -= equipmentData.Hp;
        MaxHp -= equipmentData.Hp;
        Attack -= equipmentData.Attack;
        Defense -= equipmentData.Defense;
        Strength -= equipmentData.Strength;
        Vitality -= equipmentData.Vitality;
        Intelligence -= equipmentData.Intelligence;
        Dexterity -= equipmentData.Dexterity;
    }
    #endregion
}