using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ObjectManager
{
    public bool Initialized { get; protected set; }
    public HeroParty HeroParty { get; protected set; }
    public Dictionary<int, BaseSkill> Skills { get; protected set; } // 스킬 객체를 미리 생성해놓고 계속해서 사용

    private Transform _heroRoot => GetRootTransform("@Heroes");
    private Transform _monsterRoot => GetRootTransform("@Monsters");

    public void Init()
    {
        Skills = new Dictionary<int, BaseSkill>();

        Object.DontDestroyOnLoad(_heroRoot.gameObject);

        BindSkills();

        Initialized = true;
    }

    public Transform GetRootTransform(string name)
    {
        GameObject root = GameObject.Find(name);
        if (root == null)
            root = new GameObject { name = name };

        return root.transform;
    }

    public void BindSkills()
    {
        foreach (var skillData in Managers.DataMng.SkillDataDict)
        {
            Type skillType = Type.GetType(skillData.Value.Name);
            if (skillType == null)
            {
                Debug.LogError("Failed to BindSkill: " + skillData.Value.Name);
                return;
            }

            var skill = Activator.CreateInstance(skillType) as BaseSkill;

            skill.SetInfo(skillData.Key);
            Skills[skillData.Key] = skill;
        }
    }

    public Hero SpawnHero(int heroDataId)
    {
        if (!Managers.DataMng.HeroDataDict.ContainsKey(heroDataId))
        {
            Debug.LogError($"Hero data doesn't exist. HeroDataId: {heroDataId}");
            return null;
        }

        HeroParty ??= new HeroParty();

        string className = Managers.DataMng.HeroDataDict[heroDataId].Name;
        GameObject go = Managers.ResourceMng.Instantiate($"{GlobalValues.HERO_PREFAB_PATH_ROOT}/{className}");
        Hero hero = go.GetComponent<Hero>();
        HeroParty.AddHero(hero);

        hero.SetInfo(heroDataId);
        go.transform.position = Vector3.zero;
        hero.transform.parent = _heroRoot;

        return hero;
    }

    public Monster SpawnMonster(int monsterDataId)
    {
        if (!Managers.DataMng.MonsterDataDict.ContainsKey(monsterDataId))
        {
            Debug.LogError($"Monster data doesn't exist. MonsterDataId: {monsterDataId}");
            return null;
        }

        string monsterName = Managers.DataMng.MonsterDataDict[monsterDataId].Name;
        GameObject go = Managers.ResourceMng.Instantiate($"{GlobalValues.MONSTER_PREFAB_PATH_ROOT}/{monsterName}");
        Monster monster = go.GetComponent<Monster>();

        monster.SetInfo(monsterDataId);
        go.transform.position = Vector3.zero;
        monster.transform.parent = _monsterRoot;

        return monster;
    }
    
    // 프로토타입 버전에서의 영웅 스폰: Knight 2명, Wizard 2명
    public void SpawnHeroesOnTest()
    {   
        for(int i = 0; i < 2; i++)
        {
            var hero = SpawnHero(GlobalValues.HERO_KNIGHT_ID);
            hero.EquipWeapon(GlobalValues.KNIGHT_START_WEAPON_ID);
        }

        for (int i = 0; i < 2; i++)
        {
            var hero = SpawnHero(GlobalValues.HERO_WIZARD_ID);
            hero.EquipWeapon(GlobalValues.WIZARD_START_WEAPON_ID);
        }
    }
}
