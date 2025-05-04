using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonsterSquadData
{
    public int DataId;
    public string Name;
    public List<MonsterSquad_MonsterData> Monsters;

    public bool Validate()
    {
        HashSet<(int, int)> usedPos = new HashSet<(int, int)> ();

        foreach (MonsterSquad_MonsterData monster in Monsters)
        {
            if (monster.x < 0 || monster.x >= 3 || monster.y < 0 || monster.y >= 3)
            {
                Debug.LogWarning($"MonsterSquadData {DataId} - Invalid position ({monster.x}, {monster.y})");
                return false;
            }
            if (usedPos.Contains((monster.x, monster.y)))
            {
                Debug.LogWarning($"MonsterSquadData {DataId} - Duplicate position ({monster.x}, {monster.y})");
                return false;
            }
            usedPos.Add((monster.x, monster.y));
        }
        return true;
    }
}

public class MonsterSquad_MonsterData
{
    public int DataId;
    public int x;
    public int y;
}

[Serializable]
public class MonsterSquadDataLoader : ILoader<int, MonsterSquadData>
{
    public List<MonsterSquadData> monsterSquads = new List<MonsterSquadData>();

    public Dictionary<int, MonsterSquadData> MakeDict()
    {
        Dictionary<int, MonsterSquadData> dic = new Dictionary<int, MonsterSquadData>();
        foreach (MonsterSquadData squad in monsterSquads)
        {
            if(squad.Validate()) dic.Add(squad.DataId, squad);
        }

        return dic;
    }
}
