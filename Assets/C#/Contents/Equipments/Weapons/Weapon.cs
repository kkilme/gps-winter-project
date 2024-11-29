public abstract class Weapon: Equipment
{
    public Data.WeaponData WeaponData => EquipmentData as Data.WeaponData;
    public Define.WeaponType WeaponType { get; protected set; }

    public BaseAction Skill1;
    public BaseAction Skill2;
    public BaseAction Skill3;
    
    public override void SetInfo(int templateId)
    {
        EquipmentType = Define.EquipmentType.Weapon;
        
        base.SetInfo(templateId);
        
        Skill1 = Managers.ObjectMng.Actions[WeaponData.Actions[0]];
        Skill2 = Managers.ObjectMng.Actions[WeaponData.Actions[1]];
        Skill3 = Managers.ObjectMng.Actions[WeaponData.Actions[2]];
    }
}
