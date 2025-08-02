using System.Collections.Generic;

public class Weapon : Equipment
{
    public WeaponData WeaponData => EquipmentData as WeaponData;
    public WeaponType WeaponType => WeaponData.WeaponType;
    public List<BattleSkill> Skills { get; protected set; } = new();

    public Weapon(int dataId)
    {
        SetData(dataId);
    }

    public override void SetData(int dataId)
    {
        base.SetData(dataId);

        foreach (int skillId in WeaponData.Skills)
        {
            Skills.Add(Managers.ObjectHolder.Skills[skillId]);
        }

        // 모든 무기에 공통적으로 있는 기본 스킬 추가
        Skills.Add(Managers.ObjectHolder.Skills[GlobalValues.ACTION_MOVE_ID]);
        Skills.Add(Managers.ObjectHolder.Skills[GlobalValues.ACTION_FLEE_ID]);
        Skills.Add(Managers.ObjectHolder.Skills[GlobalValues.ACTION_BAG_ID]);
    }
}
