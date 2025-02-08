public class Bite : MoveAttackAction
{
    public override void SetInfo(int templateId)
    {
        ActionTargetType = GlobalEnums.ActionTargetType.Single;
        
        base.SetInfo(templateId);
    }
}