using System.Collections.Generic;
using UnityEngine;

public class MoveTargetSelector : IActionTargetSelector
{
    private BattleGridCell _cell => Managers.BattleMng.CurrentTurnCreature.CurrentCell;
    private BattleGridCell[,] _grid => _gridSide == GlobalEnums.GridSide.HeroSide ? Managers.BattleMng.BattleGridSystem.HeroGrid : Managers.BattleMng.BattleGridSystem.EnemyGrid;
    private GlobalEnums.GridSide _gridSide;


    public MoveTargetSelector(GlobalEnums.GridSide gridSide)
    {
        _gridSide = gridSide;
    }

    public List<BattleGridCell> GetValidTargets()
    {
        List<BattleGridCell> targets = new List<BattleGridCell>();
        int row = _cell.Row;
        int col = _cell.Col;

        for(int i = 0; i<4; i++)
        {
            int nr = row + GlobalValues.DIRECTION_4WAY[i, 0];
            int nc = col + GlobalValues.DIRECTION_4WAY[i, 1];

            if (nr >= 0 && nr <= 1 && nc >= 0 && nc <= 2)
            {
                targets.Add(_grid[nr, nc]);
            }
        }

        return targets;
    }
}
