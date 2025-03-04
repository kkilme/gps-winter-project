using DG.Tweening;
using System.Collections;
using UnityEngine;

/// <summary>
/// 제자리에 서서 수행되는 액션
/// </summary>
public abstract class StandSkill : BaseSkill
{
    protected Vector3 _meleeAttackRange;

    public override IEnumerator Execute()
    {
        throw new System.NotImplementedException();
    }
}