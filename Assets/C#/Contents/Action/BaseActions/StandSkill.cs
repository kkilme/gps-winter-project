using DG.Tweening;
using System.Collections;
using UnityEngine;

/// <summary>
/// 제자리에 서서 수행되는 액션
/// </summary>
public abstract class StandSkill : BaseSkill
{
    protected Vector3 _meleeAttackRange;

    public override IEnumerator Execute(int coinHeadCount = -1)
    {
        throw new System.NotImplementedException();
    }
}