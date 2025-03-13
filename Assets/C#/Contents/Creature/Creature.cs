using DG.Tweening;
using System.Collections;
using UnityEngine;
using Data;

public abstract class Creature : MonoBehaviour
{
    public CreatureStat CreatureStat { get; protected set; }
    public CreatureData CreatureData { get; protected set; }
    public CreatureType CreatureType { get; protected set; }
    public Animator Animator { get; protected set; }
    public BattleGridCell StandingCell { get; set; }
    
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
    public virtual void SetInfo(int dataId)
    { 
        gameObject.name = $"{CreatureData.DataId}_{CreatureData.Name}";
        CreatureStat.SetStat(CreatureData);
    }

    /// <summary>
    /// 전투 씬에서 Creature가 정면을 바라보도록 함
    /// </summary>
    public abstract Tween LookOpponent(float duration = 0f);
    
    public void TakeDamage(int damage)
    {
        CreatureStat.TakeDamage(damage);
        
        if (CreatureStat.Hp <= 0)
        {
            OnDead();
            return;
        }

        // TODO - 애니메이션
    }
    
    public void OnDead()
    { 
        //Managers.ObjectMng.Despawn(CreatureType, Id);
    }

    public void OnHeal(int heal)
    {
        CreatureStat.OnHeal(heal);
    }

}
