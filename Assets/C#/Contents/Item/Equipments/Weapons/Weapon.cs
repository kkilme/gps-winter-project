using System.Collections.Generic;
using UnityEngine;

public class Weapon: Equipment
{
    public WeaponData WeaponData => EquipmentData as WeaponData;
    public WeaponType WeaponType { get; protected set; }
    public List<BaseSkill> Skills { get; protected set; } = new();
    
    public Weapon(int dataId)
    {
        SetData(dataId);
    }

    public override void SetData(int dataId)
    {
        ItemType = ItemType.Weapon;

        WeaponData weaponData = Managers.DataMng.WeaponDataDict[dataId];
        ItemData = weaponData;
        WeaponType = weaponData.WeaponType;

        base.SetData(dataId);

        foreach (int skillId in WeaponData.Skills)
        {
            Skills.Add(Managers.ObjectMng.Skills[skillId]);
        }

        Skills.Add(Managers.ObjectMng.Skills[GlobalValues.ACTION_MOVE_ID]);
        Skills.Add(Managers.ObjectMng.Skills[GlobalValues.ACTION_FLEE_ID]);
    }
}
