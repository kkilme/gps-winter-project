using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ObjectManager
{
    public bool Initialized { get; protected set; }
    public Dictionary<ulong, Hero> Heroes { get; protected set; }
    public Dictionary<ulong, Monster> Monsters { get; protected set; }
    public HeroParty HeroParty { get; protected set; }

    public ulong NextHeroId;
    public ulong NextMonsterId;

    public Dictionary<int, BaseAction> Actions { get; protected set; }

    public Transform HeroRoot => GetRootTransform("@Heroes");
    public Transform MonsterRoot => GetRootTransform("@Monsters");

    public void Init()
    {
        Heroes = new Dictionary<ulong, Hero>();
        Monsters = new Dictionary<ulong, Monster>();
        Actions = new Dictionary<int, BaseAction>();

        NextHeroId = 10000;
        NextMonsterId = 20000;

        Object.DontDestroyOnLoad(HeroRoot.gameObject);
        BindActions();

        Initialized = true;
    }

    public Transform GetRootTransform(string name)
    {
        GameObject root = GameObject.Find(name);
        if (root == null)
            root = new GameObject { name = name };

        return root.transform;
    }

    #region Bind

    public void BindActions()
    {
        foreach (var actionData in Managers.DataMng.ActionDataDict)
        {
            Type actionType = Type.GetType(actionData.Value.Name);
            if (actionType == null)
            {
                Debug.LogError("Failed to BindAction: " + actionData.Value.Name);
                return;
            }

            var action = Activator.CreateInstance(actionType) as BaseAction;

            action.SetInfo(actionData.Key);

            Actions[actionData.Key] = action;
        }
    }

    #endregion

    #region Creature
    public Hero SpawnHero(int heroDataId)
    {
        if (!Managers.DataMng.HeroDataDict.ContainsKey(heroDataId))
        {
            Debug.LogError($"No hero data exists with heroDataId: {heroDataId}");
            return null;
        }

        HeroParty ??= new HeroParty();

        string className = Managers.DataMng.HeroDataDict[heroDataId].Name;
        GameObject go = Managers.ResourceMng.Instantiate($"{Define.HERO_PATH}/{className}");
        Hero hero = go.GetComponent<Hero>();
        HeroParty.AddHero(hero);

        hero.SetInfo(heroDataId);
        go.transform.position = Vector3.zero;
        hero.transform.parent = HeroRoot;
        hero.Id = NextHeroId;
        Heroes[NextHeroId++] = hero;
        return hero;
    }

    public void SpawnHeroesOnTest()
    {   
        for(int i = 0; i < 2; i++)
        {
            var hero = SpawnHero(Define.HERO_KNIGHT_ID);
            hero.EquipWeapon(Define.KNIGHT_START_WEAPON_ID);
        }

        for (int i = 0; i < 2; i++)
        {
            var hero = SpawnHero(Define.HERO_WIZARD_ID);
            hero.EquipWeapon(Define.WIZARD_START_WEAPON_ID);
        }
    }

    public Monster SpawnMonster(int monsterDataId)
    {
        string className = Managers.DataMng.MonsterDataDict[monsterDataId].Name;
        GameObject go = Managers.ResourceMng.Instantiate($"{Define.MONSTER_PATH}/{className}");
        Monster monster = go.GetComponent<Monster>();

        monster.SetInfo(monsterDataId);
        go.transform.position = Vector3.zero;
        monster.transform.parent = MonsterRoot;
        monster.Id = NextMonsterId;
        Monsters[NextMonsterId++] = monster;

        return monster;
    }

    public void Despawn(Define.CreatureType creatureType, ulong id)
    {
        Creature creature = null;
        switch (creatureType)
        {
            case Define.CreatureType.Hero:
                creature = Heroes[id];
                Heroes.Remove(id);
                break;
            case Define.CreatureType.Monster:
                creature = Monsters[id];
                Monsters.Remove(id);
                break;
        }

        if (creature != null)
            Managers.ResourceMng.Destroy(creature.gameObject);
    }

    public Creature GetCreatureWithId(ulong id)
    {
        Creature creature = null;
        if (Managers.ObjectMng.Heroes.TryGetValue(id, out Hero hero))
            creature = hero;
        if (Managers.ObjectMng.Monsters.TryGetValue(id, out Monster monster))
            creature = monster;

        return creature;
    }


    #endregion
}
