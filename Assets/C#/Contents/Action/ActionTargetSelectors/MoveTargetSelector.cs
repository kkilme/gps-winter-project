using System.Collections.Generic;
using UnityEngine;

public class MoveTargetSelector : ActionTargetSelector
{
    public override bool NeedTargetSelection { get; protected set; } = true;
    public override void CalculateTargetableCells()
    {
        TargetableCells.Clear();
        var currentcell = Managers.BattleMng.CurrentTurnCreature.StandingCell;
        var myGrid = Managers.BattleMng.GridSystem.SideToGrid(currentcell.GridSide);

        int row = currentcell.Row;
        int col = currentcell.Column;

        for (int i = 0; i<4; i++)
        {
            int nr = row + GlobalValues.DIRECTION_4WAY[i, 0];
            int nc = col + GlobalValues.DIRECTION_4WAY[i, 1];

            if (nr >= 0 && nr <= GlobalValues.BATTLEGRID_ROW_COUNT - 1 && nc >= 0 && nc <= GlobalValues.BATTLEGRID_COL_COUNT - 1)
            {
                TargetableCells.Add(myGrid[nr, nc]);
            }
        }
    }
}
