using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Monster : Creature
{
    public CreatureAI AIBrain { get; protected set; } // 자동 전투를 구현한다면 Creature로 옮겨야 할 듯
    public Data.MonsterData MonsterData => CreatureData as Data.MonsterData;
    
    public override void SetInfo(int dataId)
    {
        CreatureType = CreatureType.Monster;
        CreatureData = Managers.DataMng.MonsterDataDict[dataId];
        AIBrain = GetComponent<CreatureAI>();
        base.SetInfo(dataId);
    }

    public override Tween LookOpponent(float duration = 0f)
    {
        return transform.DOLookAt(Managers.BattleMng.GridSystem.HeroGrid[StandingCell.Row, 2 - StandingCell.Column].transform.position, duration);
    }    
}
