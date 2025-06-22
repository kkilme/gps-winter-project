using DG.Tweening;
using System.Collections;
using UnityEngine;

public abstract class Creature : MonoBehaviour
{
    public CreatureData CreatureData { get; protected set; } // 변하지 않는 기초 데이터
    public CreatureStat CreatureStat { get; protected set; } // 장비 등에 의해 변할 수 있는 스탯
    public UI_CreatureProfile ProfileUI { get; protected set; } // Creature의 프로필 UI
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

    public void BindProfileUI(UI_CreatureProfile profileUI)
    {
        ProfileUI = profileUI;
    }

    /// <summary>
    /// 전투 씬에서 Creature가 정면을 바라보도록 함
    /// </summary>
    public abstract Tween LookFront(float duration = 0f);
    
    /// <summary>
    /// damage만큼 체력 감소
    /// </summary>
    public void TakeDamage(int damage, DamageTextType damageTextType)
    {
        if(damage > 0) Animator.SetTrigger("OnDamaged");
        if (ProfileUI) ProfileUI.OnDamaged();

        CreatureStat.TakeDamage(damage); // 실제 데미지 적용
        DamageTextFactory.CreateDamageText(this, damage, damageTextType); // 데미지 텍스트 생성

        if (IsDead())
        {
            CoroutineRunner.Instance.StartCoroutine(OnDead());
        }
    }
    
    /// <summary>
    /// heal만큼 체력 회복
    /// </summary>
    public void TakeHeal(int heal)
    {
        var finalHeal = Mathf.Min(heal, CreatureStat.FinalStat.MaxHp - CreatureStat.Hp);

        CreatureStat.TakeHeal(finalHeal);
        if (ProfileUI) ProfileUI.OnHeal();
        DamageTextFactory.CreateDamageText(this, finalHeal, DamageTextType.Heal);
    }

    /// <summary>
    /// 최대 체력의 percent만큼 회복 
    /// </summary>
    public void TakeHeal(float percent)
    {
        int heal = Mathf.FloorToInt(CreatureStat.MaxHp * percent);
        TakeHeal(heal);
    }

    public abstract IEnumerator OnDead();

    public bool IsDead()
    {
        return CreatureStat.Hp <= 0;
    }

}
