using DG.Tweening;
using System.Collections;
using UnityEngine;

/// <summary>
/// 적에게 접근 후 사용하는 스킬 (근접 스킬)
/// </summary>
public abstract class MeleeSkill : BattleSkill
{
    protected Vector3 _originalPos;
    public override IEnumerator Execute()
    {
        // 코인 던지기
        CoinTossHelper.CoinTossResult coinResult = CoinTossHelper.CoinToss(SkillData.CoinCount, Executor.CreatureStat.NameToStat(SkillData.UsingStat));

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

    // 근접 스킬이므로 대상 위치로 이동
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

    // 실제 공격 - 애니메이션 실행, 데미지 적용
    // 공격 애니메이션, 각종 스킬 효과 등을 변경하려면 override 필요
    protected virtual IEnumerator Attack(int coinHeadCount)
    {
        var attackSkillData = SkillData as AttackSkillData;
        var targets = EffectRange.GetAffectedTargets(SelectedTargetCell).FindAll(cell => cell.PlacedCreature != null);

        _animator.SetTrigger(GlobalValues.ANIMATION_PARAM_ATTACK1);

        yield return new WaitForSeconds(1f);

        var dmgTextType = attackSkillData.AttackType == AttackType.Physical ? DamageTextType.PhysicalDamage : DamageTextType.MagicDamage;
        foreach (var target in targets)
        {
            if (target.PlacedCreature == null) continue;
            var damage = DamageCalculator.CalculateFinalDamage(Executor, target.PlacedCreature, attackSkillData, coinHeadCount, targets.Count);
            target.PlacedCreature.TakeDamage(damage, dmgTextType);
        }
    }

    // 제자리로 돌아가기
    protected virtual Tween Return()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(Executor.transform.DOLookAt(_originalPos, 0.3f));

        sequence.Append(Executor.transform.DOMove(_originalPos, GameUtility.CalculateMovetime(Executor.transform.position, _originalPos))
            .OnStart(() => { _animator.SetBool(GlobalValues.ANIMATION_PARAM_MOVING, true); })
            .OnComplete(() =>
            {
                _animator.SetBool(GlobalValues.ANIMATION_PARAM_MOVING, false);
                Executor.LookFront(0.5f);
            }));

        return sequence.Play();
    }
}