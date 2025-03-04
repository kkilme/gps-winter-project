using DG.Tweening;
using System.Collections;
using UnityEngine;

public class HealPotion : BaseItem
{
    public override ActionEffectRange EffectRange { get; protected set; } = new DummyRange();
    public override ActionTargetSelector TargetSelector { get; protected set; } = new AllySelector(); // TODO
    public override IEnumerator Execute()
    {
        return base.Execute();
    }

}
