public class SampleSingleSword : Weapon
{
    public override void SetInfo(int templateId)
    {
        WeaponType = Define.WeaponType.SingleSword;
        
        base.SetInfo(templateId);
    }
}
