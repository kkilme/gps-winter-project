using System.Collections.Generic;
using UnityEngine;

public class OpponentSelector : ActionTargetSelector
{
    public override bool NeedTargetSelection { get; protected set; } = true;
    private AttackRangeType _rangeType;

    public OpponentSelector(AttackRangeType rangeType)
    {
        _rangeType = rangeType;
    }

    public override void SetTargettableCells()
    {
        TargettableCells.Clear();
        var currentcell = Managers.BattleMng.CurrentTurnCreature.StandingCell;
        var opponentGrid = Managers.BattleMng.BattleGridSystem.SideToGrid(currentcell.GridSide == GridSide.HeroSide ? GridSide.MonsterSide : GridSide.HeroSide);

        if (_rangeType == AttackRangeType.Ranged)
        {
            foreach (var cell in opponentGrid)
            {
                if (!cell.IsEmpty())
                {
                    TargettableCells.Add(cell);
                }
            }
        } else if (_rangeType == AttackRangeType.Melee)
        {
            for (int col = 0; col < GlobalValues.BATTLEGRID_COL_COUNT; col++)
            {
                for (int row = GlobalValues.BATTLEGRID_ROW_COUNT-1; row >= 0; row--)
                {
                    // 근접일 시 어떤 열에서 가장 앞에 있는 적만 타게팅 가능
                    var cell = opponentGrid[row, col];
                    if (!cell.IsEmpty())
                    {
                        TargettableCells.Add(cell);
                        break;
                    }
                }
            }
        }
    }
}
