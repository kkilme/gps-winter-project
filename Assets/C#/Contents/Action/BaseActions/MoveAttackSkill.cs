using DG.Tweening;
using System.Collections;
using UnityEngine;

/// <summary>
/// 이동 후 공격하는 액션 (근접 공격)
/// </summary>
public abstract class MoveAttackSkill : BaseSkill
{
    protected Vector3 _originalPos;
    public override IEnumerator Execute(int coinHeadCount)
    {
        yield return MoveToTarget().WaitForCompletion();
        //TODO
    }

    protected virtual Tween MoveToTarget()
    {   
        _originalPos = Owner.transform.position;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(Owner.transform.DOLookAt(SelectedTargetCell.transform.position, 0.3f));

        Vector3 direction = (Owner.transform.position - SelectedTargetCell.transform.position).normalized;

        float stopDistance = 1.5f;

        Vector3 targetPos = SelectedTargetCell.transform.position + direction * stopDistance;
        // 상대의 약간 앞까지만 이동
        sequence.Append(
            Owner.transform.DOMove(targetPos, GameUtility.CalculateMovetime(Owner.transform.position, targetPos))
            .OnStart(() => { _animator.SetBool(GlobalValues.ANIMATION_PARAM_MOVING, true); })
            .OnComplete(() => { _animator.SetBool(GlobalValues.ANIMATION_PARAM_MOVING, false); }));

        return sequence.Play();
    }

    protected virtual IEnumerator Attack(int coinHeadCount)
    {
        yield return null;
    }

    protected virtual Tween Return()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(Owner.transform.DOLookAt(_originalPos, 0.3f));

        sequence.Append(Owner.transform.DOMove(_originalPos, GameUtility.CalculateMovetime(Owner.transform.position, _originalPos))
            .OnStart(() => { _animator.SetBool(GlobalValues.ANIMATION_PARAM_MOVING, true); })
            .OnComplete(() => { 
                _animator.SetBool(GlobalValues.ANIMATION_PARAM_MOVING, false);
                Owner.LookOpponent(0.5f);
            }));

        return sequence.Play();
    }

}