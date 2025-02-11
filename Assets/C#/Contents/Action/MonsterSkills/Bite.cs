public class Bite : MoveAttackAction
{
    public override void SetInfo(int templateId)
    {
        ActionTargetType = ActionTargetType.Single;
        
        base.SetInfo(templateId);
    }
}