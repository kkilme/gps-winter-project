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
        if (IsDead()) return;
        if (damage > 0) Animator.SetTrigger("OnDamaged");
        if (ProfileUI) ProfileUI.OnDamaged();

        CreatureStat.TakeDamage(damage); // 실제 데미지 적용

        DamageTextFactory.CreateDamageText(this, damage, damageTextType); // 데미지 텍스트 생성

        if (IsDead())
        {
            CoroutineRunner.Instance.StartCoroutine(OnDead());
        }
    }

    /// <summary>
    /// 최대 체력의 ratio 비율만큼 체력 감소 (소수점은 올림처리)
    /// </summary>
    /// <remarks>ratio는 0.0f ~ 1.0f 사이의 값</remarks>
    public void TakeDamage(float ratio, DamageTextType damageTextType)
    {
        ratio = Mathf.Clamp01(ratio);
        int damage = Mathf.CeilToInt(CreatureStat.MaxHp * ratio);
        TakeDamage(damage, damageTextType);
    }

    /// <summary>
    /// amount만큼 체력 회복.
    /// </summary>
    public void TakeHeal(int amount)
    {
        var finalHeal = Mathf.Min(amount, CreatureStat.FinalStat.MaxHp - CreatureStat.Hp);

        CreatureStat.TakeHeal(finalHeal);
        if (ProfileUI) ProfileUI.OnHeal();
        DamageTextFactory.CreateDamageText(this, finalHeal, DamageTextType.Heal);
    }

    /// <summary>
    /// 최대 체력의 ratio 비율만큼 체력 회복 (소수점은 올림처리)
    /// </summary>
    /// <remarks>ratio는 0.0f ~ 1.0f 사이의 값</remarks>
    public void TakeHeal(float ratio)
    {
        ratio = Mathf.Clamp01(ratio); // 0.0f ~ 1.0f 사이로 제한
        int heal = Mathf.CeilToInt(CreatureStat.MaxHp * ratio);
        TakeHeal(heal);
    }

    public abstract IEnumerator OnDead();

    public bool IsDead()
    {
        return CreatureStat.Hp <= 0;
    }

}
