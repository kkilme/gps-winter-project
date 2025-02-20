public class Bite : MoveAttackSkill
{
    public override ActionTargetSelector TargetSelector { get; protected set; } = new SingleOpponentSelector();
    public override void SetInfo(int templateId)
    {        
        base.SetInfo(templateId);
    }
}