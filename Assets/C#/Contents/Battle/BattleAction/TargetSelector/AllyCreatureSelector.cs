using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아군 Creature를 선택하는 BattleActionTargetSelector
/// </summary>
public class AllyCreatureSelector : BattleActionTargetSelector
{
    public override bool NeedTargetSelection { get; protected set; } = true;
    public override void CalculateTargetableCells()
    {
        TargetableCells.Clear();
        var currentcell = Managers.BattleMng.CurrentTurnCreature.StandingCell;
        var allyGrid = Managers.BattleMng.GridSystem.SideToGrid(currentcell.GridSide);

        foreach (var cell in allyGrid)
        {
            if (!cell.IsEmpty())
            {
                TargetableCells.Add(cell);
            }
        }
    }
}
