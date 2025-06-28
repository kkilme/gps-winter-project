public class Charge : MeleeSkill
{
    public override ActionEffectRange EffectRange { get; protected set; } = new OneByTwoRange();
    public override BattleActionTargetSelector TargetSelector { get; protected set; } = new OpponentSelector(AttackRangeType.Melee);
}