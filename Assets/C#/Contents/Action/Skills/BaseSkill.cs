using Data;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public abstract class BaseSkill : BaseAction
{
    public SkillData SkillData { get; protected set; }

    public override void SetData(int dataId)
    {
        base.SetData(dataId);
        SkillData = Managers.DataMng.SkillDataDict[dataId];
    }
}