using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// BattleAction의 대상을 선택하는 역할을 하는 클래스
/// </summary>
public abstract class BattleActionTargetSelector
{
    /// <summary>
    /// Action의 대상이 될 수 있는 모든 BattleGridCell. CalculateTargetableCells에 의해 채워짐.
    /// </summary>
    public HashSet<BattleGridCell> TargetableCells { get; protected set; } = new();
    /// <summary>
    /// 플레이어가 Action의 대상을 지정해 줄 필요가 있는지 여부
    /// </summary>
    public abstract bool NeedTargetSelection { get; protected set; }
    /// <summary>
    /// Action의 대상이 될 수 있는 Cell 계산하여 TargetableCells에 저장
    /// </summary>
    public abstract void CalculateTargetableCells();

    /// <summary>
    /// 가능한 대상들 중 랜덤 대상 선택. 
    /// 주의: 가능한 대상이 없을 경우 null을 반환하므로, 실행 전 Action.IsExecutable()을 통해 미리 확인해야 함.
    /// </summary>
    public BattleGridCell GetRandomTarget()
    {
        if (TargetableCells.Count == 0) return null;

        return TargetableCells.ElementAt(Random.Range(0, TargetableCells.Count));
    }

    public bool IsTargetable(BattleGridCell cell)
    {
        return cell != null && TargetableCells.Contains(cell);
    }

    public virtual void OnActionUnset()
    {
        TargetableCells.Clear();
    }
}
