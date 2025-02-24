using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Monster : Creature
{
    #region Field
    
    public Data.MonsterData MonsterData => CreatureData as Data.MonsterData;
    
    #endregion

    public override void SetInfo(int templateId)
    {
        CreatureType = CreatureType.Monster;
        CreatureData = Managers.DataMng.MonsterDataDict[templateId];
        base.SetInfo(templateId);
    }

    #region Battle

    public override void DoPrepareAction()
    {
        EquipAction();
        TargetCell = ChooseTarget(); 
        
        //if (!CurrentAction.IsExecutable())
        //{
        //    CurrentAction.UnEquip();
        //    TargetCell = null;
        //    DoPrepareAction();
        //}
        
        CreatureBattleState = CreatureBattleState.ActionProceed;
    }

    public override void DoAction()
    {
        CoinHeadNum = 0;
        //((UI_BattleScene)Managers.UIMng.SceneUI).CoinTossUI.ShowCoinToss(CurrentAction, CoinHeadNum);
        
        CurrentAction.Execute();
    }

    public override void DoEndTurn()
    {
        ((UI_BattleScene)Managers.UIMng.SceneUI).CoinTossUI.EndTurn();
        
        CreatureBattleState = CreatureBattleState.Wait;
        TargetCell = null;
        Managers.BattleMng.NextTurn();
    }

    public override Tween LookOpponent(float duration = 0f)
    {
        return transform.DOLookAt(Managers.BattleMng.BattleGridSystem.HeroGrid[StandingCell.Row, 2 - StandingCell.Column].transform.position, duration);
    }

    #endregion

    // TODO - Action 선택 알고리즘 구현
    protected void EquipAction()
    {
        //int randomKey = MonsterData.Actions[Random.Range(0, MonsterData.Actions.Count)];

        //CurrentAction =  Managers.ObjectMng.Skills[randomKey];
        //CurrentAction.Equip(this);
    }
    
    // TODO - Target 선택 알고리즘 구현
    protected BattleGridCell ChooseTarget()
    {
        List<ulong> keysList = new List<ulong>(Managers.ObjectMng.Heroes.Keys);
        ulong randomKey = keysList[Random.Range(0, keysList.Count)];

        return Managers.ObjectMng.Heroes[randomKey].StandingCell;
    }
}
