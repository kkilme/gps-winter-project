using System.Collections.Generic;

/// <summary>
/// BattleAction의 영향 범위를 정의하는 클래스
/// </summary>
public abstract class BattleActionEffectRange
{
    /// <summary>
    /// 선택한 selected 셀을 기준으로 BattleAction의 영향을 받는 모든 BattleGridCell를 계산하여 반환
    /// </summary>
    public abstract List<BattleGridCell> GetAffectedTargets(BattleGridCell selected);
}