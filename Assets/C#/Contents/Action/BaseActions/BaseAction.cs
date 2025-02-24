using Data;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public abstract class BaseAction
{
    #region Field

    public int DataId { get; protected set; }
    public Creature Owner { get; set; }
    public BattleGridCell SelectedTargetCell { get; set; }
    public abstract ActionTargetSelector TargetSelector { get; protected set; }

    protected Animator _animator => Owner.Animator;
    protected int _coinHeadCount;

    #endregion
    
    public virtual void SetInfo(int dataId)
    {
        DataId = dataId;
    }
    public abstract IEnumerator Execute(int coinHeadCount = -1);


    public void OnSet()
    {
        Owner = Managers.BattleMng.CurrentTurnCreature;
        if(TargetSelector == null)
        {
            Debug.LogError("Action doesn't have targetselector: " + GetType().Name);
            return;
        }

        TargetSelector.SetTargettableCells();
        if(!TargetSelector.NeedTargetSelection)
        {
            SelectedTargetCell = TargetSelector.GetRandomTarget();
        }
    }

    public void OnUnset()
    {
        Owner = null;
        SelectedTargetCell = null;
        TargetSelector.OnActionUnset();
    }
    
    public void HighlightAffectedTargets(BattleGridCell cell)
    {
        foreach (var target in TargetSelector.GetAffectedTargets(cell))
        {
            target.HighlightAll();
        }
    }

    public void HighlightAffectedTargets()
    {
        if(SelectedTargetCell == null)
        {
            Debug.LogError("SelectedTargetCell is null");
            return;
        }
        HighlightAffectedTargets(SelectedTargetCell);
    }
}