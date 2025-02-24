using System.Collections.Generic;
using UnityEngine;

public abstract class ActionTargetSelector
{
    /// <summary>
    /// Action의 대상이 될 수 있는 모든 BattleGridCell
    /// </summary>
    public List<BattleGridCell> TargettableCells { get; protected set; } = new();
    /// <summary>
    /// 플레이어가 Action의 대상을 지정해 줄 필요가 있는지 여부
    /// </summary>
    public abstract bool NeedTargetSelection { get; protected set; }
    protected BattleGridCell _currentcell => Managers.BattleMng.CurrentTurnCreature.StandingCell;
    protected BattleGridCell[,] _myGrid => _gridSide == GridSide.HeroSide ? Managers.BattleMng.BattleGridSystem.HeroGrid : Managers.BattleMng.BattleGridSystem.MonsterGrid;
    protected BattleGridCell[,] _opponentGrid => _gridSide == GridSide.HeroSide ? Managers.BattleMng.BattleGridSystem.MonsterGrid : Managers.BattleMng.BattleGridSystem.HeroGrid;
    protected GridSide _gridSide => Managers.BattleMng.CurrentTurnCreature is Hero ? GridSide.HeroSide : GridSide.MonsterSide;
    
    /// <summary>
    /// TargettableCells 설정
    /// </summary>
    public abstract void SetTargettableCells();
    /// <summary>
    /// 선택한 Target을 기준으로 Action의 영향을 받는 모든 BattleGridCell
    /// </summary>
    public virtual List<BattleGridCell> GetAffectedTargets(BattleGridCell selected)
    {
        return new List<BattleGridCell> { selected };
    }

    public BattleGridCell GetRandomTarget()
    {
        return TargettableCells[Random.Range(0, TargettableCells.Count)];
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
