using System.Collections.Generic;
using UnityEngine;

public class Weapon: Equipment
{
    public Data.WeaponData WeaponData => EquipmentData as Data.WeaponData;
    public WeaponType WeaponType { get; protected set; }
    public List<BaseSkill> Skills { get; protected set; } = new();
    
    public override void SetInfo(int dataId)
    {
        EquipmentType = EquipmentType.Weapon;
        EquipmentData = Managers.DataMng.WeaponDataDict[dataId];
        WeaponType = Managers.DataMng.WeaponDataDict[dataId].WeaponType;

        base.SetInfo(dataId);

        foreach (int skillId in WeaponData.Skills)
        {
            Skills.Add(Managers.ObjectMng.Skills[skillId]);
        }

        Skills.Add(Managers.ObjectMng.Skills[GlobalValues.ACTION_MOVE_ID]);
        Skills.Add(Managers.ObjectMng.Skills[GlobalValues.ACTION_FLEE_ID]);
    }
}
