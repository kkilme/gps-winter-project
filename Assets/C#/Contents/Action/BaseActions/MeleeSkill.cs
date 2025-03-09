using DG.Tweening;
using System.Collections;
using UnityEngine;
using Data;

/// <summary>
/// 적에게 접근 후 공격하는 스킬 (근접 공격)
/// </summary>
public abstract class MeleeSkill : BaseSkill
{
    protected Vector3 _originalPos;
    public override IEnumerator Execute()
    {
        // 코인 던지기
        var coinResult = CoinTossser.CoinToss(SkillData.CoinCount, Owner.CreatureStat.NameToStat(SkillData.UsingStat));

        // 코인 던지기 UI 애니메이션 재생
        yield return Managers.BattleMng.BattleSceneUI.CoinTossDisplay.ShowResult(coinResult.result);

        // 타겟으로 이동
        yield return MoveToTarget().WaitForCompletion();

        // 공격
        yield return Attack(coinResult.successCount);
        
        // 제자리로 복귀
        yield return Return().WaitForCompletion();

        Managers.BattleMng.OnActionEnd();
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
        var attackSkillData = SkillData as AttackSkillData;
        var target = SelectedTargetCell.PlacedCreature;

        var damage = DamageCalculator.CalculateFinalDamage(Owner, target, attackSkillData, coinHeadCount);

        _animator.SetTrigger(GlobalValues.ANIMATION_PARAM_ATTACK);
        SelectedTargetCell.PlacedCreature.TakeDamage(damage);

        yield return DOVirtual.DelayedCall(_animator.GetCurrentAnimatorStateInfo(0).length + 0.2f, () => { }).WaitForCompletion();
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