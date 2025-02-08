using System.Collections.Generic;

public interface IMultipleTargetSelector: IActionTargetSelector
{
    /// <summary>
    /// 선택한 Target을 기준으로 Action의 영향을 받는 모든 BattleGridCell
    /// </summary>
    public List<BattleGridCell> GetAffectedTargets(BattleGridCell selected);
}
