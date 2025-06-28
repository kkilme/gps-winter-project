using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

// TODO: ObjectManager 제거
public class ObjectManager
{
    public bool Initialized { get; protected set; }
    public Dictionary<int, BattleSkill> Skills { get; protected set; } // 스킬 객체를 미리 생성해놓고 반복해서 사용
    private Transform _monsterRoot => GlobalUtility.FindOrCreateTransform("@Monsters");

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

    public Monster SpawnMonster(int monsterDataId)
    {
        if (!Managers.DataMng.MonsterDataDict.ContainsKey(monsterDataId))
        {
            Debug.LogError($"Monster data doesn't exist. MonsterDataId: {monsterDataId}");
            return null;
        }

        string monsterName = Managers.DataMng.MonsterDataDict[monsterDataId].Name;
        GameObject go = Managers.ResourceMng.Instantiate(GlobalValues.MONSTER_PREFAB_PATH_PREFIX + monsterName);
        Monster monster = go.GetComponent<Monster>();

        monster.SetData(monsterDataId);
        go.transform.position = Vector3.zero;
        monster.transform.parent = _monsterRoot;

        return monster;
    }
}
