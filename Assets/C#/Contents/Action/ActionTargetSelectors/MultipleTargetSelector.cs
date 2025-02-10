using System.Collections.Generic;

public abstract class MultipleTargetSelector: ActionTargetSelector
{
    /// <summary>
    /// 선택한 Target을 기준으로 Action의 영향을 받는 모든 BattleGridCell
    /// </summary>
    public abstract List<BattleGridCell> GetAffectedTargets(BattleGridCell selected);
}
