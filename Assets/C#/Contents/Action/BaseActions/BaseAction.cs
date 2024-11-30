using DG.Tweening;
using UnityEngine;

public abstract class BaseAction
{
    #region Field

    public int DataId { get; protected set; }
    public Data.ActionData ActionData { get; protected set; }
    
    public Creature Owner { get; set; }
    public Animator Animator => Owner.Animator;
    public BattleGridCell TargetCell => Owner.TargetCell;
    
    public Define.ActionTargetType ActionTargetType { get; protected set; }
    public Define.Stat UsingStat { get; protected set; } = Define.Stat.Strength;

    public int CoinNum { get; protected set; }
    public int CoinHeadNum { get; protected set; }

    #endregion
    
    public virtual void SetInfo(int templateId)
    {
        DataId = templateId;
        ActionData = Managers.DataMng.ActionDataDict[templateId];
        
        CoinNum = ActionData.CoinNum;
    }

    public void Equip(Creature owner)
    {
        Owner = owner;
    }
    
    public void UnEquip()
    {
        Owner.CurrentAction = null;
        Owner = null;
    }

    public int CoinToss()
    {
        if (ActionData.UsingStat == Define.Stat.None)
            return -1;

        int coinHeadNum = 0;
        if (Owner.CreatureType == Define.CreatureType.Hero)
        {
            for (int i = 0; i < CoinNum; i++)
            {
                float value = Random.value;
                if (value < ((Hero)Owner).HeroStat.GetStatByDefine(UsingStat) / 100f)
                    coinHeadNum++;
            }
        }
        else
        {
            for (int i = 0; i < CoinNum; i++)
            {
                float value = Random.value;
                if (value < ((Monster)Owner).MonsterData.Stat / 100f)
                    coinHeadNum++;
            }
        }

        return coinHeadNum;
    }
    public abstract bool CanStartAction();
    
    #region Action

    public void DoAction()
    {
        CoinHeadNum = CoinToss();
        ((UI_BattleScene)Managers.UIMng.SceneUI).CoinTossUI.ShowCoinToss(this, CoinToss());

        Owner.transform.DOLookAt(TargetCell.transform.position, 0.3f,  AxisConstraint.None, new Vector3(0, 1, 0)).OnComplete(OnStartAction);
    }
    
    public abstract void OnStartAction();
    
    public abstract void OnHandleAction();

    public virtual void OnJumpFWDStart()
    {
    }

    public virtual void OnJumpFWDEnd()
    {
    }
    
    public virtual void OnJumpBWDStart()
    {
    }
    
    public virtual void OnMoveStart()
    {
    }
    
    public virtual void OnMoveFWDEnd()
    {
    }
    
    public virtual void OnMoveBWDStart()
    {
    }
    
    public virtual void OnAttackEnd()
    {
    }
    
    public void OnActionEnd()
    {
        Animator.Play("Idle");
        
        Vector3 front;
        if (Owner.CreatureType == Define.CreatureType.Hero)
            front = new Vector3(0, 0, 1);
        else
            front = new Vector3(0, 0, -1);

        Owner.transform.DOLookAt(front, 0.3f, AxisConstraint.None, new Vector3(0, 1, 0)).OnComplete(Owner.DoEndTurn);
    }

    #endregion
    
}