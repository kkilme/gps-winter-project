using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Move : BaseSkill
{
    public override ActionTargetSelector TargetSelector { get; protected set; } = new MoveTargetSelector();
    public override void SetInfo(int templateId)
    {
        base.SetInfo(templateId);
    }

    public override IEnumerator Execute(int coinHeadCount = -1)
    {
        throw new System.NotImplementedException();
    }
}