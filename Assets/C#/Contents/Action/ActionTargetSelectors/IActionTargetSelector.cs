using System.Collections.Generic;

public interface IActionTargetSelector
{
    ///<summary>
    /// Action의 대상이 될 수 있는 모든 BattleGridCell
    ///</summary>
    public List<BattleGridCell> GetValidTargets();
}
