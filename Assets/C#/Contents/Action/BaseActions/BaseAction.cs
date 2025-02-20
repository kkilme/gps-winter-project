using Data;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public abstract class BaseAction
{
    #region Field

    public int DataId { get; protected set; }
    public Creature Owner { get; set; }
    public BattleGridCell TargetCell { get; set; }
    public abstract ActionTargetSelector TargetSelector { get; protected set; }

    protected Animator _animator => Owner.Animator;
    protected int _coinHeadCount;

    #endregion
    
    public virtual void SetInfo(int dataId)
    {
        DataId = dataId;
    }

    public void OnSet()
    {
        Owner = Managers.BattleMng.CurrentTurnCreature;
        if(TargetSelector == null)
        {
            Debug.LogError("Action doesn't have targetselector: " + GetType().Name);
            return;
        }

        TargetSelector.SetTargettableCells();
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

    public virtual bool IsExecutable()
    {
        return TargetSelector.IsTargettable(TargetCell);
    }

    public abstract IEnumerator Execute(int coinHeadCount = -1);
    
    public void OnActionEnd()
    {
        _animator.Play("Idle");
        
        Vector3 front;
        if (Owner.CreatureType == CreatureType.Hero)
            front = new Vector3(0, 0, 1);
        else
            front = new Vector3(0, 0, -1);

        Owner.transform.DOLookAt(front, 0.3f, AxisConstraint.None, new Vector3(0, 1, 0)).OnComplete(Owner.DoEndTurn);
    }
}