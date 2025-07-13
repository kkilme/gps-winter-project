using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Monster))]
public class OrcAI : MonsterAI
{
    private int _berserkSpinCooldown = 0;

    private const int BERSERKSPIN_COLLDOWN = 1;

    public override BattleSkill DecideSkill()
    {
        List<BattleSkill> skillList = new List<BattleSkill>(_skillList);

        var berserkSpin = skillList.Find(s => s is BerserkSpin) as BerserkSpin;
        if(_berserkSpinCooldown == 0 && berserkSpin != null)
        {
            SetupBerserkSpin(berserkSpin);
            return berserkSpin;
        }

        _berserkSpinCooldown = Mathf.Max(0, _berserkSpinCooldown-1);
        if(berserkSpin != null) skillList.Remove(berserkSpin);

        while (skillList.Count > 0)
        {
            var skill = skillList[Random.Range(0, skillList.Count)];
            if (skill.IsExecutable(_monster))
            {
                skill.Set(_monster);
                skill.SetRandomTarget();
                return skill;
            }
            else
            {
                skillList.Remove(skill);
            }
        }

        // 아무 스킬도 선택할 수 없다면 DummySkill을 반환하여 턴을 넘김
        return new DummySkill();
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
            if(maxTargetCount == range.GetAffectedTargets(targetable).Count) candidates.Add(targetable);
        }

        berserkSpin.SetTarget(candidates[Random.Range(0, candidates.Count)]);
        _berserkSpinCooldown = BERSERKSPIN_COLLDOWN;
    }
    
}
