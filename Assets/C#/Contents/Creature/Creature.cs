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
    public abstract Tween LookFront(float duration = 0f);
    
    public void TakeDamage(int damage, DamageTextType damageTextType)
    {
        if(damage > 0) Animator.SetTrigger("OnDamaged");
        CreatureStat.TakeDamage(damage);
        DamageTextFactory.CreateDamageText(this, damage, damageTextType);

        if (IsDead())
        {
            CoroutineRunner.Instance.StartCoroutine(OnDead());
        }
    }
    
    public IEnumerator OnDead()
    {
        Animator.SetBool(GlobalValues.ANIMATION_PARAM_DEAD, true);
        if(Managers.SceneMng.CurrentScene.SceneType == SceneType.BattleScene)
        {
            Managers.BattleMng.RemoveCreature(this);
        }
        yield return new WaitForSeconds(5f);
        Managers.ResourceMng.Destroy(gameObject);
        //Managers.ObjectMng.Despawn(CreatureType, Id);
    }

    public void TakeHeal(int heal)
    {
        CreatureStat.TakeHeal(heal);
        DamageTextFactory.CreateDamageText(this, heal, DamageTextType.Heal);
    }

    public bool IsDead()
    {
        return CreatureStat.Hp <= 0;
    }

}
