using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Monster))]
public class DefaultMonsterAI : CreatureAI
{
    private Monster _monster;
    private List<BaseSkill> _skillList;

    public override void Init()
    {
        base.Init();
        _monster = _owner as Monster;
        _skillList = new List<BaseSkill>();
        foreach(int dataId in _monster.MonsterData.Actions)
        {
            _skillList.Add(Managers.ObjectMng.Skills[dataId]);
        }
    }

    // 몬스터는 기본적으로 가진 스킬 중 하나를 랜덤으로 선택함.
    // 스킬 대상 또한 랜덤으로 선택함.
    // 더 디테일한 AI 구현시, CreatureAI를 상속받는 다른 클래스 제작하기.
    public override BaseSkill DecideSkill()
    {
        List<BaseSkill> skillList = new List<BaseSkill>(_skillList);

        while (skillList.Count > 0)
        {
            var skill = skillList[Random.Range(0, skillList.Count)];
            if (skill.IsExecutable(_monster))
            {
                skill.Set(_monster);
                skill.SetRandomTarget();
                return skill;
            } else
            {
                skillList.Remove(skill);
            }
        }
        
        return new DummySkill();
    }
}
