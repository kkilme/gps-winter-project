using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ObjectHolder
{
    public bool Initialized { get; protected set; }
    public Dictionary<int, BattleSkill> Skills { get; protected set; } // 스킬 객체를 미리 생성해놓고 반복해서 사용

    public void Init()
    {
        Skills = new Dictionary<int, BattleSkill>();

        BindSkills();

        Initialized = true;
    }

    private void BindSkills()
    {
        foreach (var skillData in Managers.DataMng.SkillDataDict)
        {
            Type skillType = Type.GetType(skillData.Value.ClassName);
            if (skillType == null)
            {
                Debug.LogError("Failed to BindSkill: " + skillData.Value.ClassName);
                continue;
            }

            var skill = Activator.CreateInstance(skillType) as BattleSkill;

            skill.SetData(skillData.Key);
            Skills[skillData.Key] = skill;
        }
    }
}
