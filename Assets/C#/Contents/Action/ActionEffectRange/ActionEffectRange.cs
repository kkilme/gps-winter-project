using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ActionEffectRange
{
    /// <summary>
    /// 선택한 Target을 기준으로 Action의 영향을 받는 모든 BattleGridCell
    /// </summary>
    public abstract List<BattleGridCell> GetAffectedTargets(BattleGridCell selected);
}
