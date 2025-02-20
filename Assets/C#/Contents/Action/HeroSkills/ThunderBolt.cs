public class ThunderBolt : StandSkill
{
    public override ActionTargetSelector TargetSelector { get; protected set; } = new SingleOpponentSelector();

}
