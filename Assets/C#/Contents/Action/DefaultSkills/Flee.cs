using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Flee : BaseSkill
{
    public override ActionEffectRange EffectRange { get; protected set; } = new DummyRange();
    public override ActionTargetSelector TargetSelector { get; protected set; } = new OpponentSelector(AttackRangeType.Melee); // TODO
    public override void SetInfo(int dataId)
    {
        base.SetInfo(dataId);
    }

    public override IEnumerator Execute()
    {
        throw new System.NotImplementedException();
    }
}