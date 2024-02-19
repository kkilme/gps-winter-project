using UnityEngine;

public class SampleSingleSword : Weapon
{
    public override void SetInfo(int templateId)
    {
        WeaponType = Define.WeaponType.SingleSword;
        
        base.SetInfo(templateId);

        Skill1 = new Strike();
        Skill1.SetInfo(Define.SKILL_STRIKE_ID, Owner);
        Skill2 = new Strike();
        Skill2.SetInfo(Define.SKILL_STRIKE_ID, Owner);
        Skill3 = new Strike();
        Skill3.SetInfo(Define.SKILL_STRIKE_ID, Owner);
    }
}