using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class MonsterSquadData
    {
        public int DataId;
        public string Name;
        public List<MonsterSquad_MonsterData> Monsters;
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
                dic.Add(squad.DataId, squad);

            return dic;
        }
    }
}