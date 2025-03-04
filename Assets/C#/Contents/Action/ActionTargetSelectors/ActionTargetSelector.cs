using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ActionTargetSelector
{
    /// <summary>
    /// Action의 대상이 될 수 있는 모든 BattleGridCell
    /// </summary>
    public HashSet<BattleGridCell> TargettableCells { get; protected set; } = new();
    /// <summary>
    /// 플레이어가 Action의 대상을 지정해 줄 필요가 있는지 여부
    /// </summary>
    public abstract bool NeedTargetSelection { get; protected set; }
    /// <summary>
    /// Action의 대상이 될 수 있는 Cell 계산
    /// </summary>
    public abstract void SetTargettableCells();
    public BattleGridCell GetRandomTarget()
    {
        return TargettableCells.ElementAt(Random.Range(0, TargettableCells.Count));
    }

    public bool IsTargettable(BattleGridCell cell)
    {
        return cell != null && TargettableCells.Contains(cell);
    }

    public virtual void OnActionUnset()
    {
        TargettableCells.Clear();
    }
}
