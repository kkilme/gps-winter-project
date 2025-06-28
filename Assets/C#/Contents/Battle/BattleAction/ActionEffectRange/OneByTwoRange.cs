using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OneByTwoRange : ActionEffectRange
{
    public override List<BattleGridCell> GetAffectedTargets(BattleGridCell selected)
    {
        List<BattleGridCell> affected = new List<BattleGridCell>() { selected };

        var x = selected.Column;
        var y = selected.Row;

        var grid = Managers.BattleMng.GridSystem.SideToGrid(selected.GridSide);

        if (y != 0 && grid[y - 1, x].PlacedCreature != null) affected.Add(grid[y - 1, x]);

        return affected;
    }
}
