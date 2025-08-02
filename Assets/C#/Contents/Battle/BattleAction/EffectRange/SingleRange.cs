using System.Collections.Generic;


public class SingleRange : BattleActionEffectRange
{
    public override List<BattleGridCell> GetAffectedTargets(BattleGridCell selected)
    {
        return new List<BattleGridCell> { selected };
    }
}
