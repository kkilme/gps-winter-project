public class ThunderBolt : StandSkill
{
    public override ActionEffectRange EffectRange { get; protected set; } = new SingleRange();
    public override BattleActionTargetSelector TargetSelector { get; protected set; } = new OpponentSelector(AttackRangeType.Ranged);
}
