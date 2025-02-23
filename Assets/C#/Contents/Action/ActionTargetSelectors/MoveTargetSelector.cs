using System.Collections.Generic;
using UnityEngine;

public class MoveTargetSelector : ActionTargetSelector
{
    public override bool NeedTargetSelection { get; protected set; } = true;
    public override void SetTargettableCells()
    {
        List<BattleGridCell> targets = new List<BattleGridCell>();

        int row = _currentcell.Row;
        int col = _currentcell.Column;

        for (int i = 0; i<4; i++)
        {
            int nr = row + GlobalValues.DIRECTION_4WAY[i, 0];
            int nc = col + GlobalValues.DIRECTION_4WAY[i, 1];

            if (nr >= 0 && nr <= 1 && nc >= 0 && nc <= 2)
            {
                targets.Add(_myGrid[nr, nc]);
            }
        }

        TargettableCells = targets;
    }
}
