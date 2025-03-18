public class ThunderBolt : StandSkill
{
    public override ActionEffectRange EffectRange { get; protected set; } = new SingleRange();
    public override ActionTargetSelector TargetSelector { get; protected set; } = new OpponentSelector(AttackRangeType.Ranged);
}
