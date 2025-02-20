using DG.Tweening;
using System.Collections;
using UnityEngine;

public class HealPotion : BaseItem
{
    public override ActionTargetSelector TargetSelector { get; protected set; } = new SingleAllySelector();
    public override IEnumerator Execute(int coinHeadCount = -1)
    {
        return base.Execute(coinHeadCount);
    }

}
