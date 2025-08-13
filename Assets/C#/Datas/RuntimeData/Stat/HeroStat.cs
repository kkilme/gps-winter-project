using UnityEngine;

public class HeroStat : CreatureStat
{
    public int HeroInstanceId { get; set; } // 이 스탯을 가지는 Hero의 InstanceId
    public override StatLayer FinalStat => BaseStat + EquipmentStat + BuffStat + DebuffStat;
    protected StatLayer EquipmentStat;

    public HeroStat(CreatureData creatureData, int heroInstanceId) : base(creatureData)
    {
        EquipmentStat = new StatLayer();
        HeroInstanceId = heroInstanceId;
    }

    /// <summary>
    /// BaseStat의 특정 스탯을 delta만큼 증가 또는 감소시킴.
    /// </summary>
    public void ModifyBaseStat(StatName stat, int delta)
    {
        switch (stat)
        {
            case StatName.BaseDamage:
                BaseStat.BaseDamage += delta;
                break;
            case StatName.MaxHp:
                BaseStat.MaxHp += delta;
                break;
            case StatName.PhysicalDefense:
                BaseStat.PhysicalDefense += delta;
                break;
            case StatName.MagicDefense:
                BaseStat.MagicDefense += delta;
                break;
            case StatName.Strength:
                BaseStat.Strength += delta;
                break;
            case StatName.Vitality:
                BaseStat.Vitality += delta;
                break;
            case StatName.Intelligence:
                BaseStat.Intelligence += delta;
                break;
            case StatName.Dexterity:
                BaseStat.Dexterity += delta;
                break;
            default:
                Debug.LogWarning($"Unknown stat: {stat}");
                return;
        }

        OnStatChanged?.Invoke(this);
    }

    /// <summary>
    /// BaseStat 이외의 스탯 초기화
    /// </summary>
    public void ClearBonusStats()
    {
        EquipmentStat = new StatLayer();
        BuffStat = new StatLayer();
        DebuffStat = new StatLayer();
    }

    /// <summary>
    /// 버프 및 디버프 스탯 초기화
    /// </summary>
    public void ClearBuffAndDebuffStats()
    {
        BuffStat = new StatLayer();
        DebuffStat = new StatLayer();
    }

    public void AddEquipmentStat(EquipmentData equipment)
    {
        EquipmentStat.MaxHp += equipment.Hp;
        EquipmentStat.BaseDamage += equipment.Attack;
        EquipmentStat.PhysicalDefense += equipment.PhysicalDefense;
        EquipmentStat.MagicDefense += equipment.MagicDefense;
        EquipmentStat.Strength += equipment.Strength;
        EquipmentStat.Vitality += equipment.Vitality;
        EquipmentStat.Intelligence += equipment.Intelligence;
        EquipmentStat.Dexterity += equipment.Dexterity;

        OnStatChanged?.Invoke(this);
    }

    public void RemoveEquipmentStat(EquipmentData equipment)
    {
        EquipmentStat.MaxHp -= equipment.Hp;
        EquipmentStat.BaseDamage -= equipment.Attack;
        EquipmentStat.PhysicalDefense -= equipment.PhysicalDefense;
        EquipmentStat.MagicDefense -= equipment.MagicDefense;
        EquipmentStat.Strength -= equipment.Strength;
        EquipmentStat.Vitality -= equipment.Vitality;
        EquipmentStat.Intelligence -= equipment.Intelligence;
        EquipmentStat.Dexterity -= equipment.Dexterity;

        OnStatChanged?.Invoke(this);
    }
}
