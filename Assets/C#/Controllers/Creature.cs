using DG.Tweening;
using System.Collections;
using UnityEngine;
using static UnityEditor.Recorder.OutputPath;
using static UnityEngine.GraphicsBuffer;

public abstract class Creature : MonoBehaviour
{
    #region Field

    public ulong Id { get; set; }
    public int DataId { get; protected set; }
    public CreatureStat CreatureStat { get; protected set; }
    public Data.CreatureData CreatureData { get; protected set; }
    public CreatureType CreatureType { get; protected set; }
    public Animator Animator { get; protected set; }
    public BattleGridCell StandingCell { get; set; }
    
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
    }

    #region Battle
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
        
        Managers.ObjectMng.Despawn(CreatureType, Id);
    }

    public void OnHeal(int heal)
    {
        CreatureStat.OnHeal(heal);
    }

    #endregion
}
