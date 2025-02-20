using Data;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public abstract class BaseSkill : BaseAction
{
    public SkillData SkillData { get; protected set; }
    public StatName UsingStat { get; protected set; }

    public override void SetInfo(int dataId)
    {
        base.SetInfo(dataId);
        SkillData = Managers.DataMng.SkillDataDict[dataId];
    }
}