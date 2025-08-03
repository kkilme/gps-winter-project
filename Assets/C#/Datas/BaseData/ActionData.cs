using System;
using System.Collections.Generic;


[Serializable]
public class ActionData
{
    public int DataId;
    public string Name;
    public string ClassName; // BaseAction을 상속받는 클래스명
    public string Description;
    public ActionDataType Type; // ActionData를 상속받는 클래스명
}

[Serializable]
public class SkillData : ActionData
{
    public int CoinCount;
    public StatName UsingStat;
    public string IconPath;
}

[Serializable]
public class AttackSkillData : SkillData
{
    public AttackType AttackType;
    public int DamagePerCoin;
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
            skill.ClassName ??= skill.Name;
            dic.Add(skill.DataId, skill);
        }
        return dic;
    }
}