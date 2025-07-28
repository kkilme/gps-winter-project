using System.Collections;
using UnityEngine;

public class Punch : MeleeSkill
{
    public override BattleActionEffectRange EffectRange { get; protected set; } = new SingleRange();
    public override BattleActionTargetSelector TargetSelector { get; protected set; } = new OpponentSelector(AttackRangeType.Melee);
}

