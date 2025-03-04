using Data;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public abstract class BaseAction
{
    #region Field
    public int DataId { get; protected set; }
    public Creature Owner { get; set; }
    protected Animator _animator => Owner.Animator;
    public BattleGridCell SelectedTargetCell { get; set; }
    public abstract ActionTargetSelector TargetSelector { get; protected set; }
    public abstract ActionEffectRange EffectRange { get; protected set; }

    #endregion
    
    public virtual void SetInfo(int dataId)
    {
        DataId = dataId;
    }

    /// <summary>
    /// 액션 수행 로직
    /// </summary>
    /// <returns></returns>
    public abstract IEnumerator Execute();

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

    // 임의의 cell 기준으로 영향 받는 대상들을 모두 하이라이트
    public void HighlightAffectedTargets(BattleGridCell cell)
    {
        foreach (var target in EffectRange.GetAffectedTargets(cell))
        {
            target.HighlightAll();
        }
    }

    // 현재 선택된 cell 기준으로 영향 받는 대상들을 모두 하이라이트
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