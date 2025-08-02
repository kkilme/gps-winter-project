using System.Collections;
using UnityEngine;

/// <summary>
/// 아무것도 하지 않는 더미 스킬
/// </summary>
public class DummySkill : BattleSkill
{
    public override BattleActionEffectRange EffectRange { get; protected set; } = new DummyRange();
    public override BattleActionTargetSelector TargetSelector { get; protected set; } = new DummySelector();

    public override IEnumerator Execute()
    {
        yield return new WaitForSeconds(1f);

        Managers.BattleMng.OnActionEnd();
    }
}