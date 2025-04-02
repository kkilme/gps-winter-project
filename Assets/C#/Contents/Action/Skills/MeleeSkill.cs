using DG.Tweening;
using System.Collections;
using UnityEngine;

/// <summary>
/// 적에게 접근 후 공격하는 스킬 (근접 공격)
/// </summary>
public abstract class MeleeSkill : BaseSkill
{
    protected Vector3 _originalPos;
    public override IEnumerator Execute()
    {
        // 코인 던지기
        var coinResult = CoinTossser.CoinToss(SkillData.CoinCount, Executor.CreatureStat.NameToStat(SkillData.UsingStat));

        // 코인 던지기 UI 애니메이션 재생
        yield return Managers.BattleMng.UI.CoinTossDisplay.ShowResult(coinResult.result, SkillData.UsingStat);

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
        _originalPos = Executor.transform.position;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(Executor.transform.DOLookAt(SelectedTargetCell.transform.position, 0.3f));

        Vector3 direction = (Executor.transform.position - SelectedTargetCell.transform.position).normalized;

        float stopDistance = 1.5f;

        Vector3 targetPos = SelectedTargetCell.transform.position + direction * stopDistance;
        // 상대의 약간 앞까지만 이동
        sequence.Append(
            Executor.transform.DOMove(targetPos, GameUtility.CalculateMovetime(Executor.transform.position, targetPos))
            .OnStart(() => { _animator.SetBool(GlobalValues.ANIMATION_PARAM_MOVING, true); })
            .OnComplete(() => { _animator.SetBool(GlobalValues.ANIMATION_PARAM_MOVING, false); }));

        return sequence.Play();
    }

    // 공격 애니메이션, 각종 스킬 효과 등을 변경하려면 override 필요
    protected virtual IEnumerator Attack(int coinHeadCount)
    {
        var attackSkillData = SkillData as AttackSkillData;
        var targets = EffectRange.GetAffectedTargets(SelectedTargetCell);

        _animator.SetTrigger(GlobalValues.ANIMATION_PARAM_ATTACK1);

        yield return DOVirtual.DelayedCall(_animator.GetCurrentAnimatorStateInfo(0).length + 0.2f, () => { }).WaitForCompletion();

        var dmgTextType = attackSkillData.AttackType == AttackType.Physical ? DamageTextType.PhysicalDamage : DamageTextType.MagicDamage;
        foreach(var target in targets)
        {
            // target.PlacedCreature이 null이 아니라는 보증은 EffectRange.GetAffectedTargets에서 함
            var damage = DamageCalculator.CalculateFinalDamage(Executor, target.PlacedCreature, attackSkillData, coinHeadCount, targets.Count);
            SelectedTargetCell.PlacedCreature.TakeDamage(damage, dmgTextType);
        }
    }

    protected virtual Tween Return()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(Executor.transform.DOLookAt(_originalPos, 0.3f));

        sequence.Append(Executor.transform.DOMove(_originalPos, GameUtility.CalculateMovetime(Executor.transform.position, _originalPos))
            .OnStart(() => { _animator.SetBool(GlobalValues.ANIMATION_PARAM_MOVING, true); })
            .OnComplete(() => { 
                _animator.SetBool(GlobalValues.ANIMATION_PARAM_MOVING, false);
                Executor.LookFront(0.5f);
            }));

        return sequence.Play();
    }
}