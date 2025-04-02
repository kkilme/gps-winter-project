
using DG.Tweening;
using System.Collections;
using UnityEngine;

/// <summary>
/// 제자리에 서서 수행되는 액션
/// </summary>
public abstract class StandSkill : BaseSkill
{
    public override IEnumerator Execute()
    {
        // 상대 보기
        yield return Executor.transform.DOLookAt(SelectedTargetCell.transform.position, 0.3f).WaitForCompletion();

        // 코인 던지기
        var coinResult = CoinTossser.CoinToss(SkillData.CoinCount, Executor.CreatureStat.NameToStat(SkillData.UsingStat));

        // 코인 던지기 UI 애니메이션 재생
        yield return Managers.BattleMng.UI.CoinTossDisplay.ShowResult(coinResult.result, SkillData.UsingStat);

        // 공격
        yield return Attack(coinResult.successCount);

        // 정면 보기
        Executor.LookFront(0.3f);

        Managers.BattleMng.OnActionEnd();
    }

    // 공격 애니메이션, 각종 스킬 효과 등을 변경하려면 override 필요
    protected virtual IEnumerator Attack(int coinHeadCount)
    {
        var attackSkillData = SkillData as AttackSkillData;
        var targets = EffectRange.GetAffectedTargets(SelectedTargetCell);

        _animator.SetTrigger(GlobalValues.ANIMATION_PARAM_ATTACK1);

        yield return DOVirtual.DelayedCall(_animator.GetCurrentAnimatorStateInfo(0).length + 0.2f, () => { }).WaitForCompletion();

        var dmgTextType = attackSkillData.AttackType == AttackType.Physical ? DamageTextType.PhysicalDamage : DamageTextType.MagicDamage;
        foreach (var target in targets)
        {
            // target.PlacedCreature이 null이 아니라는 보증은 EffectRange.GetAffectedTargets에서 함
            var damage = DamageCalculator.CalculateFinalDamage(Executor, target.PlacedCreature, attackSkillData, coinHeadCount, targets.Count);
            SelectedTargetCell.PlacedCreature.TakeDamage(damage, dmgTextType);
        }
    }
}