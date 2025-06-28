using DG.Tweening;
using System.Collections;
using UnityEngine;

/// <summary>
/// 다른 셀로 이동하는 기본 스킬. 해당 셀에 다른 Creature가 있을 경우, 위치를 교환함.
/// </summary>
public class Move : BattleSkill
{
    public override ActionEffectRange EffectRange { get; protected set; } = new SingleRange();
    public override BattleActionTargetSelector TargetSelector { get; protected set; } = new MoveTargetSelector();

    public override IEnumerator Execute()
    {
        Sequence sequence = DOTween.Sequence();

        // 해당 셀에 다른 Creature가 있을 경우, 위치를 교환
        if (!SelectedTargetCell.IsEmpty())
        {   
            Creature targetCreature = SelectedTargetCell.PlacedCreature;
            Animator targetAnimator = targetCreature.Animator;

            sequence.Append(Executor.transform.DOLookAt(SelectedTargetCell.transform.position, 0.1f));
            sequence.Join(targetCreature.transform.DOLookAt(Executor.transform.position, 0.1f));

            sequence.Append(
                Executor.transform.DOMove(SelectedTargetCell.transform.position, GameUtility.CalculateMovetime(Executor.transform, SelectedTargetCell.transform))
                .OnStart(() => { _animator.SetBool(GlobalValues.ANIMATION_PARAM_MOVING, true); })
                .OnComplete(() => { _animator.SetBool(GlobalValues.ANIMATION_PARAM_MOVING, false); })
             );

            sequence.Join(
                targetCreature.transform.DOMove(Executor.transform.position, GameUtility.CalculateMovetime(targetCreature.transform, Executor.transform))
                .OnStart(() => { targetAnimator.SetBool(GlobalValues.ANIMATION_PARAM_MOVING, true); })
                .OnComplete(() => { targetAnimator.SetBool(GlobalValues.ANIMATION_PARAM_MOVING, false); })
             );
        } else // 해당 셀이 비어있을 경우, 그냥 이동
        {
            sequence.Append(Executor.transform.DOLookAt(SelectedTargetCell.transform.position, 0.1f));
            sequence.Append(
                Executor.transform.DOMove(SelectedTargetCell.transform.position, GameUtility.CalculateMovetime(Executor.transform, SelectedTargetCell.transform))
                .OnStart(() => { _animator.SetBool(GlobalValues.ANIMATION_PARAM_MOVING, true); })
                .OnComplete(() => { _animator.SetBool(GlobalValues.ANIMATION_PARAM_MOVING, false); })
             );
        }

        // 실제 이동
        yield return sequence.Play().WaitForCompletion();
        
        // BattleGridSystem에 변동된 위치 적용 및 Creature가 다시 정면을 바라보게 함
        if (!SelectedTargetCell.IsEmpty())
        {   
            Creature targetCreature = SelectedTargetCell.PlacedCreature;
            Managers.BattleMng.GridSystem.SwapCreaturePosition(Executor, SelectedTargetCell.PlacedCreature);

            sequence = DOTween.Sequence();
            sequence.Append(Executor.LookFront(0.1f));
            sequence.Join(targetCreature.LookFront(0.1f));

            yield return sequence.Play().WaitForCompletion();
        } else
        {
            Managers.BattleMng.GridSystem.MoveCreature(Executor, SelectedTargetCell);
            yield return Executor.LookFront(0.1f).WaitForCompletion();
        }

        Managers.BattleMng.OnActionEnd();
    }
}