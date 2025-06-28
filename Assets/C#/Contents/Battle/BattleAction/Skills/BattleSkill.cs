using DG.Tweening;
using System.Collections;
using UnityEngine;

public abstract class BattleSkill : BattleAction
{
    public int DataId { get; protected set; }
    public SkillData SkillData { get; protected set; }

    public void SetData(int dataId)
    {
        DataId = dataId;
        SkillData = Managers.DataMng.SkillDataDict[dataId];
    }
}