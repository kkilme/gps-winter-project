using DG.Tweening;
using System.Collections;
using UnityEngine;

public class HealPotionAction : ItemAction
{
    public override ActionEffectRange EffectRange { get; protected set; } = new SingleRange();
    public override BattleActionTargetSelector TargetSelector { get; protected set; } = new AllySelector();

    public override IEnumerator Execute()
    {
        yield return null;
    }

}
