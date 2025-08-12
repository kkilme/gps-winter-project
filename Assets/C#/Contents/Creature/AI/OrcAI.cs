using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Monster))]
public class OrcAI : MonsterAI
{
    private int _berserkSpinCooldown = 0;

    private const int BERSERKSPIN_COOLDOWN = 1;

    public override BattleSkill DecideSkill()
    {
        List<BattleSkill> skillList = new List<BattleSkill>(_skillList);

        var berserkSpin = skillList.Find(s => s is BerserkSpin) as BerserkSpin;
        if (_berserkSpinCooldown == 0 && berserkSpin != null)
        {
            SetupBerserkSpin(berserkSpin);
            return berserkSpin;
        }

        _berserkSpinCooldown = Math.Max(0, _berserkSpinCooldown - 1);
        if (berserkSpin != null) skillList.Remove(berserkSpin);

        return SelectRandomSkill(skillList);
    }

    /// <summary>
    /// BerserkSpin 스킬 타겟 지정 등 초기화
    /// </summary>
    private void SetupBerserkSpin(in BerserkSpin berserkSpin)
    {
        berserkSpin.Set(_monster);
        berserkSpin.SetRandomTarget();
        var targetables = berserkSpin.TargetSelector.TargetableCells;

        BerserkSpinEffectRange range = berserkSpin.EffectRange as BerserkSpinEffectRange;

        int maxTargetCount = 0;

        foreach (var targetable in targetables)
        {
            maxTargetCount = Mathf.Max(maxTargetCount, range.GetAffectedTargets(targetable).Count);
        }

        List<BattleGridCell> candidates = new List<BattleGridCell>();
        foreach (var targetable in targetables)
        {
            // 최대로 많은 적을 공격할 수 있는 타겟을 지정한다.
            if (maxTargetCount == range.GetAffectedTargets(targetable).Count) candidates.Add(targetable);
        }

        berserkSpin.SetTarget(candidates.GetRandomElement());
        _berserkSpinCooldown = BERSERKSPIN_COOLDOWN;
    }

}
