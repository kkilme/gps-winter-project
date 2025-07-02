using DG.Tweening;
using System.Collections;
using UnityEngine;

public class HealPotionAction : ItemAction
{
    public override ActionEffectRange EffectRange { get; protected set; } = new SingleRange();
    public override BattleActionTargetSelector TargetSelector { get; protected set; } = new SelfSelector();

    public override IEnumerator Execute()
    {
        SelectedTargetCell.PlacedCreature.TakeHeal(.2f);
        Managers.BattleMng.OnActionEnd();
        yield return null;
    }
}