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
            if (monster.col < 0 || monster.col >= 3 || monster.row < 0 || monster.row >= 3)
            {
                Debug.LogWarning($"MonsterSquadData {DataId} - Invalid position ({monster.col}, {monster.row})");
                return false;
            }
            if (usedPos.Contains((monster.col, monster.row)))
            {
                Debug.LogWarning($"MonsterSquadData {DataId} - Duplicate position ({monster.col}, {monster.row})");
                return false;
            }
            usedPos.Add((monster.col, monster.row));
        }
        return true;
    }
}

public class MonsterSquad_MonsterData
{
    public int DataId;
    public int col;
    public int row;
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
