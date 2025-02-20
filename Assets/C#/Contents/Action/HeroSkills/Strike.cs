using UnityEngine;

public class Strike : MoveAttackSkill
{
    public override ActionTargetSelector TargetSelector { get; protected set; } = new SingleOpponentSelector();
}
