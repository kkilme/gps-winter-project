using System.Collections.Generic;
using UnityEngine;

public class Weapon: Equipment
{
    public Data.WeaponData WeaponData => EquipmentData as Data.WeaponData;
    public Define.WeaponType WeaponType { get; protected set; }
    public List<BaseAction> Actions { get; protected set; } = new();
    
    public override void SetInfo(int dataId)
    {
        EquipmentType = Define.EquipmentType.Weapon;
        EquipmentData = Managers.DataMng.WeaponDataDict[dataId];
        WeaponType = Managers.DataMng.WeaponDataDict[dataId].WeaponType;

        base.SetInfo(dataId);

        foreach (int actionId in WeaponData.Actions)
        {
            Actions.Add(Managers.ObjectMng.Actions[actionId]);
        }

        Actions.Add(Managers.ObjectMng.Actions[Define.ACTION_MOVE_ID]);
        Actions.Add(Managers.ObjectMng.Actions[Define.ACTION_FLEE_ID]);
    }
}
