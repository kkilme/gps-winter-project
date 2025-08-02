using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHolder
{
    public bool Initialized { get; protected set; }
    public Dictionary<int, BattleSkill> Skills { get; protected set; } // 미리 생성해놓고 반복해서 사용
                                                                       // 객체를 하나씩만 생성하여 반복 사용하기 때문에,
                                                                       // 전투에서 한 턴에 한 Creature만 Action을 실행하는 것이 보장되지 않거나
                                                                       // 스킬별로 어떠한 '상태'를 가지게 된다면
                                                                       // 풀링 + 팩토리 패턴같은 구조로 수정이 필요함.

    public Dictionary<int, AreaEncounter> Encounters { get; protected set; }

    public void Init()
    {
        Skills = new Dictionary<int, BattleSkill>();
        Encounters = new Dictionary<int, AreaEncounter>();

        BindSkills();
        BindEncounters();

        Initialized = true;
    }

    private void BindSkills()
    {
        foreach (var skillData in Managers.DataMng.SkillDataDict)
        {
            Type skillType = Type.GetType(skillData.Value.ClassName);
            if (skillType == null || !typeof(BattleSkill).IsAssignableFrom(skillType))
            {
                Debug.LogError("Failed to Bind Skill: " + skillData.Value.ClassName);
                continue;
            }

            var skill = Activator.CreateInstance(skillType) as BattleSkill;

            skill.SetData(skillData.Key);
            Skills[skillData.Key] = skill;
        }
    }

    private void BindEncounters()
    {
        foreach (var encounterData in Managers.DataMng.EncounterDataDict)
        {
            Type encounterType = Type.GetType(encounterData.Value.ClassName);
            if (encounterType == null || !typeof(AreaEncounter).IsAssignableFrom(encounterType))
            {
                Debug.LogError("Failed to Bind Encounter: " + encounterData.Value.ClassName);
                continue;
            }

            var encounter = Activator.CreateInstance(encounterType) as AreaEncounter;
            encounter.SetData(encounterData.Value);
            Encounters[encounterData.Key] = encounter;
        }
    }
}
