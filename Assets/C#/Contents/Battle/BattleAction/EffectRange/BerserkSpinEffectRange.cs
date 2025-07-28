using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BerserkSpin 스킬의 영향 범위. Orc(스킬 사용 Creature)가 근접하여 다가간 위치를 기준으로, 근처의 모든 셀(최대 8개).
/// </summary>
public class BerserkSpinEffectRange : BattleActionEffectRange
{
    public override List<BattleGridCell> GetAffectedTargets(BattleGridCell selected)
    {
        List<BattleGridCell> affected = new List<BattleGridCell>();

        var col = selected.Column;
        var row = selected.Row + 1;

        var grid = Managers.BattleMng.GridSystem.SideToGrid(selected.GridSide);

        (int r, int c)[] candidates = new (int, int)[8] {
            (row-1, col-1), (row, col - 1), (row - 1, col), (row+1, col+1), (row, col + 1), (row + 1, col), (row + 1, col - 1), (row - 1, col + 1)
        };
        foreach (var candidate in candidates)
        {
            int r = candidate.r;
            int c = candidate.c;
            if (r < 0 || c < 0 || r >= GlobalValues.BATTLEGRID_ROW_COUNT || c >= GlobalValues.BATTLEGRID_COL_COUNT || grid[r, c].PlacedCreature == null) continue;
            affected.Add(grid[r, c]);
        }

        return affected;
    }
}
