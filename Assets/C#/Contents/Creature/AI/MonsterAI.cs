using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Monster))]
public class MonsterAI : CreatureAI
{
    protected Monster _monster;
    protected List<BattleSkill> _skillList; // 몬스터가 보유한 스킬

    public override void Init()
    {
        base.Init();
        _monster = _owner as Monster;
        _skillList = new List<BattleSkill>();
        foreach (int dataId in _monster.MonsterData.Actions)
        {
            _skillList.Add(Managers.ObjectHolder.Skills[dataId]);
        }
    }

    // 몬스터는 기본적으로 가진 스킬 중 하나를 랜덤으로 선택함.
    // 스킬 대상 또한 랜덤으로 선택함.
    // 더 디테일한 AI 구현시, 오버라이딩 이용하기.
    public override BattleSkill DecideSkill()
    {
        List<BattleSkill> skillList = new List<BattleSkill>(_skillList);

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

}
