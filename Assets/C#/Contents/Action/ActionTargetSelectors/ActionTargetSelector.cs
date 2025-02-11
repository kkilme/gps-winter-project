using System.Collections.Generic;

public abstract class ActionTargetSelector
{
    /// <summary>
    /// Action의 대상이 될 수 있는 모든 BattleGridCell
    /// </summary>
    public List<BattleGridCell> TargettableCells { get; protected set; } = new();
    protected BattleGridCell _currentcell => Managers.BattleMng.CurrentTurnCreature.CurrentCell;
    protected BattleGridCell[,] _myGrid => _gridSide == GridSide.HeroSide ? Managers.BattleMng.BattleGridSystem.HeroGrid : Managers.BattleMng.BattleGridSystem.MonsterGrid;
    protected BattleGridCell[,] _opponentGrid => _gridSide == GridSide.HeroSide ? Managers.BattleMng.BattleGridSystem.MonsterGrid : Managers.BattleMng.BattleGridSystem.HeroGrid;
    protected GridSide _gridSide => Managers.BattleMng.CurrentTurnCreature is Hero ? GridSide.HeroSide : GridSide.MonsterSide;
    
    /// <summary>
    /// TargettableCells 설정
    /// </summary>
    public abstract void SetTargettableCells();
}
