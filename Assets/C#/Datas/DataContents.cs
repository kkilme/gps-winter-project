using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    #region CreatureData
    [Serializable]
    public class CreatureData
    {
        public int DataId;
        public string Name;
        public int Hp;
        public int Attack;
        public int PhysicalDefense;
        public int MagicDefense;
    }
    #endregion

    #region HeroData

    [Serializable]
    public class HeroData : CreatureData
    {
        public int Strength;
        public int Vitality;
        public int Intelligence;
        public int Dexterity;
    }

    [Serializable]
    public class HeroDataLoader : ILoader<int, HeroData>
    {
        public List<HeroData> heroes = new List<HeroData>();

        public Dictionary<int, HeroData> MakeDict()
        {
            Dictionary<int, HeroData> dic = new Dictionary<int, HeroData>();
            foreach (HeroData hero in heroes)
                dic.Add(hero.DataId, hero);

            return dic;
        }
    }

    #endregion

    #region MonsterData

    [Serializable]
    public class MonsterData : CreatureData
    {
        public int Stat;
        public int Speed;
        public List<int> Actions = new List<int>();
    }

    [Serializable]
    public class MonsterDataLoader : ILoader<int, MonsterData>
    {
        public List<MonsterData> monsters = new List<MonsterData>();

        public Dictionary<int, MonsterData> MakeDict()
        {
            Dictionary<int, MonsterData> dic = new Dictionary<int, MonsterData>();
            foreach (MonsterData monster in monsters)
                dic.Add(monster.DataId, monster);

            return dic;
        }
    }

    #endregion

    #region MonsterSquadData
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
    #endregion

    #region ItemData

    [Serializable]
    public class ItemData
    {
        public int DataId;
        public string Name;
        public string Description;
        public int Heal;
    }

    [Serializable]
    public class ItemDataLoader : ILoader<int, ItemData>
    {
        public List<ItemData> items = new List<ItemData>();

        public Dictionary<int, ItemData> MakeDict()
        {
            var dic = new Dictionary<int, ItemData>();
            foreach (ItemData item in items)
                dic.Add(item.DataId, item);

            return dic;
        }
    }
    #endregion

    #region EquipmentData
    [Serializable]
    public class EquipmentData
    {
        public int DataId;
        public string Name;
        public int Hp;
        public int Attack;
        public int PhysicalDefense;
        public int MagicDefense;
        public int Dexterity;
        public int Strength;
        public int Vitality;
        public int Intelligence;
    }
    #endregion

    #region WeaponData
    [Serializable]
    public class WeaponData : EquipmentData
    {
        public int LeftIndex;
        public int RightIndex;
        public Define.WeaponType WeaponType;
        public List<int> Actions;
    }

    [Serializable]
    public class WeaponDataLoader : ILoader<int, WeaponData>

    {
        public List<WeaponData> weapons = new List<WeaponData>();

        public Dictionary<int, WeaponData> MakeDict()
        {
            var dic = new Dictionary<int, WeaponData>();
            foreach (var weapon in weapons)
                dic.Add(weapon.DataId, weapon);

            return dic;
        }
    }
    #endregion

    #region ArmorData
    [Serializable]
    public class ArmorData : EquipmentData
    {
        public int ArmorIndex;
    }

    [Serializable]
    public class ArmorDataLoader : ILoader<int, ArmorData>
    {
        public List<ArmorData> armors = new List<ArmorData>();

        public Dictionary<int, ArmorData> MakeDict()
        {
            var dic = new Dictionary<int, ArmorData>();
            foreach (var armor in armors)
                dic.Add(armor.DataId, armor);

            return dic;
        }
    }
    #endregion

    #region ActionData
    [Serializable]
    public class ActionData
    {
        public int DataId;
        public Define.ActionDataType Type; // ActionData 또는 이를 상속받는 클래스명
        public string Name;
        public string Description;
        public int CoinCount;
        public Define.Stat UsingStat;
        public string IconName;
    }

    [Serializable]
    public class AttackActionData : ActionData
    {
        public Define.AttackType AttackType;
        public int DamagePerCoin;
    }

    [Serializable]
    public class ActionDataLoader : ILoader<int, ActionData>
    {
        public List<ActionData> actions = new List<ActionData>();

        public Dictionary<int, ActionData> MakeDict()
        {
            var dic = new Dictionary<int, ActionData>();
            foreach (var action in actions)
            {
                Type actionType = Type.GetType("Data."+ action.Type.ToString());
                if (actionType == null)
                {
                    Debug.LogError($"Failed to get type: " + action.Type);
                    return null;
                }
                string json = JsonConvert.SerializeObject(action);
                ActionData actionData = JsonConvert.DeserializeObject(json, actionType) as ActionData; // DeserializeObject는 object타입을 반환하기 때문에 ActionData로 캐스팅
                dic.Add(action.DataId, actionData);
            }

            return dic;
        }
    }
    #endregion

    #region AreaData
    [Serializable]
    public class AreaData
    {
        public string Name; // AreaName enum의 값과 같아야 함
        public string BattleFieldName; // 전투 필드의 프리팹 이름
    }

    [Serializable]
    public class AreaDataSet : ILoader<Define.AreaName, AreaData>
    {
        public List<AreaData> areadatas = new();

        public Dictionary<Define.AreaName, AreaData> MakeDict()
        {
            var dic = new Dictionary<Define.AreaName, AreaData>();
            foreach (AreaData areadata in areadatas)
            {
                if (Enum.TryParse(areadata.Name, out Define.AreaName areaName))
                {
                    dic.Add(areaName, areadata);
                }
                else
                {
                    Debug.LogError($"{areadata.Name} - AreaName is invalid!");
                }
            }
            return dic;
        }
    }
    #endregion

    #region QuestData

    [Serializable]
    public class QuestReward
    {
        public int RewardDataId;
        public int Quantity;
    }

    [Serializable]
    public class QuestData
    {
        public int DataId;
        public string Name;
        public string Description;

        public string AreaName; // 퀘스트 수락 시 이동되는 Area. AreaName enum의 값과 같아야함.
        public QuestReward[] Rewards;
        public int[] UnlockQuestDataId; // 해당 퀘스트 완료 시 열리는 퀘스트의 DataId

        public bool IsRepeatable; // 퀘스트 반복 가능 여부
        public bool IsUnlocked; // 퀘스트 개방 여부
        public bool IsComplete; // 퀘스트 완료 여부. 처음은 무조건 false.
    }

    [Serializable]
    public class QuestDataLoader : ILoader<int, QuestData>
    {
        public List<QuestData> quests = new List<QuestData>();

        public Dictionary<int, QuestData> MakeDict()
        {
            var dic = new Dictionary<int, QuestData>();
            foreach (QuestData quest in quests)
            {
                if (!Enum.TryParse(quest.AreaName, out Define.AreaName areaName))
                {
                    Debug.LogError($"Quest {quest.DataId} - AreaName is invalid!");
                    continue;
                }
                dic.Add(quest.DataId, quest);
            }
            return dic;
        }
    }

    #endregion
}