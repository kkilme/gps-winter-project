using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SingleRange : ActionEffectRange
{
    public override List<BattleGridCell> GetAffectedTargets(BattleGridCell selected)
    {
        return new List<BattleGridCell> { selected };
    }
}
