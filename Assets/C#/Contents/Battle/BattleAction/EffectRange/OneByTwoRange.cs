using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 대상 기준 1 x 2 범위
/// </summary>
public class OneByTwoRange : BattleActionEffectRange
{
    public override List<BattleGridCell> GetAffectedTargets(BattleGridCell selected)
    {
        List<BattleGridCell> affected = new List<BattleGridCell>() { selected };

        var x = selected.Column;
        var y = selected.Row;

        var grid = Managers.BattleMng.GridSystem.SideToGrid(selected.GridSide);

        // placed creature가 null이 아닌 셀만 대상으로 함
        // null인 셀도 필요하다면 새로운 Range를 만들어야 할듯
        if (y != 0 && grid[y - 1, x].PlacedCreature != null) affected.Add(grid[y - 1, x]);

        return affected;
    }
}
