using System.Collections.Generic;

public abstract class ActionTargetSelector
{
    // Action의 대상이 될 수 있는 모든 BattleGridCell
    public List<BattleGridCell> TargettableCells { get; protected set; } = new();
    protected BattleGridCell _currentcell => Managers.BattleMng.CurrentTurnCreature.CurrentCell;
    protected BattleGridCell[,] _grid => _gridSide == GlobalEnums.GridSide.HeroSide ? Managers.BattleMng.BattleGridSystem.HeroGrid : Managers.BattleMng.BattleGridSystem.EnemyGrid;
    protected GlobalEnums.GridSide _gridSide => Managers.BattleMng.CurrentTurnCreature is Hero ? GlobalEnums.GridSide.HeroSide : GlobalEnums.GridSide.EnemySide;
    
    // TargettableCells 설정
    public abstract void CalculateValidTargets();
}
