using DG.Tweening;
using System.Collections;
using UnityEngine;

public abstract class Creature : MonoBehaviour
{
    public CreatureData CreatureData { get; protected set; } // 변하지 않는 기초 데이터
    public CreatureStat CreatureStat { get; protected set; } // 장비 등에 의해 변할 수 있는 스탯
    public Animator Animator { get; protected set; }
    public BattleGridCell StandingCell { get; set; }
    
    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        Animator = gameObject.GetOrAddComponent<Animator>();
    }

    // 수동 실행
    public abstract void SetData(int dataId);

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
