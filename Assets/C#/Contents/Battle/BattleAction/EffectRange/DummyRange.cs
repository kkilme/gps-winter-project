using System.Collections.Generic;

/// <summary>
/// 대상 범위가 필요 없는 액션용 더미 클래스
/// </summary>
public class DummyRange : BattleActionEffectRange
{
    public override List<BattleGridCell> GetAffectedTargets(BattleGridCell selected)
    {
        return new List<BattleGridCell>();
    }
}
