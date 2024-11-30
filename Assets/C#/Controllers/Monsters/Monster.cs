using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : Creature
{
    #region Field
    
    public Data.MonsterData MonsterData => CreatureData as Data.MonsterData;
    public MonsterStat MonsterStat => (MonsterStat)CreatureStat;
    
    #endregion

    public override void SetInfo(int templateId)
    {
        CreatureType = Define.CreatureType.Monster;
        
        base.SetInfo(templateId);
    }

    #region Battle

    public override void DoPrepareAction()
    {
        EquipAction();
        TargetCell = ChooseTarget(); 
        
        if (!CurrentAction.CanStartAction())
        {
            CurrentAction.UnEquip();
            TargetCell = null;
            DoPrepareAction();
        }
        
        CreatureBattleState = Define.CreatureBattleState.ActionProceed;
    }

    public override void DoAction()
    {
        CoinHeadNum = 0;
        CoinHeadNum = CurrentAction.CoinToss();
        ((UI_BattleScene)Managers.UIMng.SceneUI).CoinTossUI.ShowCoinToss(CurrentAction, CoinHeadNum);
        
        CurrentAction.DoAction();
    }

    public override void DoEndTurn()
    {
        ((UI_BattleScene)Managers.UIMng.SceneUI).CoinTossUI.EndTurn();
        
        CreatureBattleState = Define.CreatureBattleState.Wait;
        CurrentAction.UnEquip();
        TargetCell = null;
        Managers.BattleMng.NextTurn();
    }
    
    #endregion

    // TODO - Action 선택 알고리즘 구현
    protected void EquipAction()
    {
        int randomKey = MonsterData.Actions[Random.Range(0, MonsterData.Actions.Count)];

        CurrentAction =  Managers.ObjectMng.Actions[randomKey];
        CurrentAction.Equip(this);
    }
    
    // TODO - Target 선택 알고리즘 구현
    protected BattleGridCell ChooseTarget()
    {
        List<ulong> keysList = new List<ulong>(Managers.ObjectMng.Heroes.Keys);
        ulong randomKey = keysList[Random.Range(0, keysList.Count)];

        return Managers.ObjectMng.Heroes[randomKey].Cell;
    }
}
