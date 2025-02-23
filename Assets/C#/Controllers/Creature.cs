using DG.Tweening;
using System.Collections;
using UnityEngine;
using static UnityEditor.Recorder.OutputPath;
using static UnityEngine.GraphicsBuffer;

public abstract class Creature : MonoBehaviour
{
    #region Field
    
    public Animator Animator { get; protected set; }
    public CreatureStat CreatureStat { get; protected set; }

    public ulong Id { get; set; }
    public int DataId { get; protected set; }
    public CreatureType CreatureType { get; protected set; }
    public Data.CreatureData CreatureData { get; protected set; }
    
    private CreatureBattleState _creatureBattleState;
    public CreatureBattleState CreatureBattleState
    {
        get => _creatureBattleState;
        set
        {
            if (_creatureBattleState == value)
                return;
            
            _creatureBattleState = value;
            switch (value)
            {
                case CreatureBattleState.Wait:
                    break;
                case CreatureBattleState.PrepareAction:
                    DoPrepareAction();
                    break;
                case CreatureBattleState.ActionProceed:
                    DoAction();
                    break;
                case CreatureBattleState.Dead:
                    break;
            }
        }
    }
    
    public BattleGridCell CurrentCell { get; set; }

    public BaseAction CurrentAction { get; set; }
    public BattleGridCell TargetCell { get; protected set; }
    public int CoinHeadNum { get; set; }
    
    #endregion
    
    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        Animator = gameObject.GetOrAddComponent<Animator>();
        CreatureStat = gameObject.GetOrAddComponent<CreatureStat>();
    }
    
    // 수동 실행
    public virtual void SetInfo(int templateId)
    {
        DataId = templateId;

        gameObject.name = $"{CreatureData.DataId}_{CreatureData.Name}";
        
        CreatureStat.SetStat(CreatureData);
        CreatureBattleState = CreatureBattleState.Wait;

        CoinHeadNum = 0;
    }

    #region Battle

    public abstract void DoPrepareAction();

    public abstract void DoAction();

    public abstract void DoEndTurn();
    /// <summary>
    /// 전투 씬에서 Creature가 정면을 바라보도록 함
    /// </summary>
    public abstract Tween LookOpponent(float duration = 0f);
    
    #endregion
    
    #region Event

    // TODO - 코인 앞면 수에 비례한 데미지 계산 
    public void OnDamage(int damage, int attackCount = 1)
    {
        CreatureStat.OnDamage(damage, attackCount);
        
        if (CreatureStat.Hp <= 0)
        {
            OnDead();
            return;
        }
        
        Debug.Log($"{CreatureStat.Name}: {CreatureStat.Hp}");

        // TODO - 애니메이션
    }
    
    public void OnDead()
    {
        CreatureBattleState = CreatureBattleState.Dead;
        
        Managers.ObjectMng.Despawn(CreatureType, Id);
    }

    public void OnHeal(int heal)
    {
        CreatureStat.OnHeal(heal);
    }

    #endregion
}
