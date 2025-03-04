using UnityEngine;

public class Strike : MeleeSkill
{
    public override ActionEffectRange EffectRange { get; protected set; } = new SingleRange();
    public override ActionTargetSelector TargetSelector { get; protected set; } = new OpponentSelector(AttackRangeType.Melee);
}
