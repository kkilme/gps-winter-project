using System.Collections.Generic;
using UnityEngine;

public class Weapon: Equipment
{
    public WeaponData WeaponData => EquipmentData as WeaponData;
    public WeaponType WeaponType { get; protected set; }
    public List<BattleSkill> Skills { get; protected set; } = new();
    
    public Weapon(int dataId)
    {
        SetData(dataId);
    }

    public override void SetData(int dataId)
    {
        base.SetData(dataId);

        WeaponType = WeaponData.WeaponType;

        foreach (int skillId in WeaponData.Skills)
        {
            Skills.Add(Managers.ObjectMng.Skills[skillId]);
        }

        // 모든 무기에 공통적으로 있는 기본 스킬 추가
        Skills.Add(Managers.ObjectMng.Skills[GlobalValues.ACTION_MOVE_ID]);
        Skills.Add(Managers.ObjectMng.Skills[GlobalValues.ACTION_FLEE_ID]);
        Skills.Add(Managers.ObjectMng.Skills[GlobalValues.ACTION_BAG_ID]);
    }
}
