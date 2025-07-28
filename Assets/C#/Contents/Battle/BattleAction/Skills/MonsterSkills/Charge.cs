public class Charge : MeleeSkill
{
    public override BattleActionEffectRange EffectRange { get; protected set; } = new OneByTwoRange();
    public override BattleActionTargetSelector TargetSelector { get; protected set; } = new OpponentSelector(AttackRangeType.Melee);
}