using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Flee : BaseSkill
{
    public override ActionTargetSelector TargetSelector { get; protected set; } = new SingleOpponentSelector(); // TODO
    public override void SetInfo(int dataId)
    {
        base.SetInfo(dataId);
    }

    public override IEnumerator Execute(int coinHeadCount = -1)
    {
        throw new System.NotImplementedException();
    }
}