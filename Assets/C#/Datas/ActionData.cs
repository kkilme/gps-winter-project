using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class ActionData
    {
        public int DataId;
        public string Name;
        public string Description;
        public string IconPath;
        public ActionDataType Type; // ActionData를 상속받는 클래스명
    }

    [Serializable]
    public class SkillData : ActionData
    {
        public int CoinCount;
        public StatName UsingStat;
    }

    [Serializable]
    public class AttackSkillData : SkillData
    {
        public AttackType AttackType;
        public int DamagePerCoin;
    }
    // TODO: ItemData
    [Serializable]
    public class ItemData : ActionData
    {
        public int Heal;
    }

    [Serializable]
    public class SkillDataLoader : ILoader<int, SkillData>
    {
        public List<SkillData> skills = new List<SkillData>();
        public Dictionary<int, SkillData> MakeDict()
        {
            var dic = new Dictionary<int, SkillData>();
            foreach (var skill in skills)
            {
                dic.Add(skill.DataId, skill);
            }
            return dic;
        }
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
}