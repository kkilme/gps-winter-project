using DG.Tweening;
using System.Collections;
using UnityEngine;

// 전투에서 Creature가 자신의 턴에 취할 수 있는 행동
public abstract class BattleAction
{
    #region Field
    public Creature Executor { get; protected set; } // 이 액션을 실행하는 Creature.
                                                     // 현재 구조에선 BattleAction 객체를 하나씩만 생성하여(ItemAction제외) 반복 사용하기 때문에,
                                                     // 전투에서 한 턴에 한 Creature만 Action을 실행하는 것이 보장되어야 함.
    public BattleGridCell SelectedTargetCell { get; protected set; }
    public abstract BattleActionTargetSelector TargetSelector { get; protected set; } // abstract로 하여 자식이 구현(값 설정)을 반강제화함. 완전한 강제는 아니므로 결국 개발자의 몫.
    public abstract ActionEffectRange EffectRange { get; protected set; }
    protected Animator _animator => Executor.Animator;

    #endregion

    /// <summary>
    /// 액션 수행 로직
    /// </summary>
    public abstract IEnumerator Execute();

    /// <summary>
    /// 이 액션을 실행할 Creature를 설정 및 타겟 가능한 셀을 계산함.
    /// </summary>
    public void Set(Creature creature)
    {
        Executor = creature;

        TargetSelector.CalculateTargetableCells();
        if(!TargetSelector.NeedTargetSelection)
        {
            SetRandomTarget();
        }
    }

    /// <summary>
    /// 바인딩된 Creature(Executor)를 해제 및 각종 상태를 초기화함.
    /// </summary>
    public void Unset()
    {
        Executor = null;
        SelectedTargetCell = null;
        TargetSelector.OnActionUnset();
    }

    public void SetTarget(BattleGridCell target)
    {
        SelectedTargetCell = target;
    }

    /// <summary>
    /// 랜덤 타겟 선택.
    /// 이는 단순히 여러 가능한 타겟 중 1개의 타겟을 랜덤으로 선택할 때 뿐만이 아니라 TargetSelector에서 선택된 단 1개의 대상을 선택하는 데에도 사용됨.
    /// </summary>
    public void SetRandomTarget()
    {
        SelectedTargetCell = TargetSelector.GetRandomTarget();
    }

    /// <summary>
    /// 액션이 실행가능한지 여부를 반환.
    /// </summary>
    public bool IsExecutable()
    {
        return TargetSelector.TargetableCells.Count > 0 || TargetSelector is DummySelector;
    }

    /// <summary>
    /// creature가 이 액션을 실행 가능한지 여부를 반환. Set과 Unset을 내부적으로 실행함.
    /// </summary>
    public bool IsExecutable(Creature creature)
    {
        Set(creature);
        bool result = IsExecutable();
        Unset();
        return result;
    }

    /// <summary>
    /// parameter의 cell 기준으로 영향 받는 대상들을 모두 하이라이트
    /// </summary>
    public void HighlightAffectedTargets(BattleGridCell cell)
    {
        foreach (var target in EffectRange.GetAffectedTargets(cell))
        {
            target.HighlightAll();
        }
    }

    /// <summary>
    /// 현재 대상으로 선택된 cell 기준으로 영향 받는 대상들을 모두 하이라이트
    /// </summary>
    public void HighlightAffectedTargets()
    {
        if(SelectedTargetCell == null)
        {
            return;
        }
        HighlightAffectedTargets(SelectedTargetCell);
    }
}