using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Orc의 스킬
/// </summary>
public class BerserkSpin : MeleeSkill
{
    public override BattleActionEffectRange EffectRange { get; protected set; } = new BerserkSpinEffectRange();
    public override BattleActionTargetSelector TargetSelector { get; protected set; } = new OpponentSelector(AttackRangeType.Melee);

    protected override IEnumerator Attack(int coinHeadCount)
    {
        var attackSkillData = SkillData as AttackSkillData;
        var targets = EffectRange.GetAffectedTargets(SelectedTargetCell).FindAll(cell => cell.PlacedCreature != null);

        _animator.SetTrigger(GlobalValues.ANIMATION_PARAM_ATTACK2);
        yield return new WaitForSeconds(.2f);

        // 이 스킬은 총 4번의 연속 공격으로 이루어짐
        // damageToTarget 딕셔너리 - key: 타겟, value (Item1: 타겟에 가해야 하는 총 데미지(totalDamage), Item2: 매 틱마다 가할 데미지)
        // value를 이렇게 나누는 이유는, 데미지가 int형이기 때문에, 매 틱마다 단순히 totalDamage/4를 가할 시 실제 가해지는 데미지가 이론상 가해져야 할 데미지보다 적을 수 있음
        // ex) 총 29 데미지를 가해야할 시 단순히 29/4 = 7데미지씩 가하면 1데미지가 소실됨
        // 해결: 1~3틱에서는 totalDamage에서 틱마다 가하는 데미지(29/4 = 7)를 감소시키고, 마지막 틱에는 남은 totalDamage(8)를 가하도록 함
        Dictionary<BattleGridCell, (int totalDamage, int tickDamage)> damageToTarget = new Dictionary<BattleGridCell, (int, int)>();

        var dmgTextType = DamageTextType.PhysicalDamage;
        foreach (var target in targets)
        {
            var damage = DamageCalculator.CalculateFinalDamage(Executor, target.PlacedCreature, attackSkillData, coinHeadCount, targets.Count);
            damageToTarget[target] = (damage, damage / 4);
        }

        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(.35f);
            foreach (var target in targets)
            {
                if (target.PlacedCreature == null) continue;
                if (target.PlacedCreature.IsDead()) continue;
                int tickDamage = damageToTarget[target].tickDamage;
                target.PlacedCreature.TakeDamage(tickDamage, dmgTextType);
                damageToTarget[target] = (damageToTarget[target].totalDamage - tickDamage, tickDamage);
            }
        }

        yield return new WaitForSeconds(.35f);
        foreach (var target in targets)
        {
            if (target.PlacedCreature == null) continue;
            if (target.PlacedCreature.IsDead()) continue;
            target.PlacedCreature.TakeDamage(damageToTarget[target].totalDamage, dmgTextType);
        }
    }
}