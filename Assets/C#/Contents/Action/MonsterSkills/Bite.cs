public class Bite : MoveAttackAction
{
    public override void SetInfo(int templateId)
    {
        ActionTargetType = Define.ActionTargetType.Single;
        
        base.SetInfo(templateId);
    }
}