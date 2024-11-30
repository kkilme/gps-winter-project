using System;
using UnityEngine;

public class CreatureStat : MonoBehaviour
{
    private string _name;
    private int _hp;
    private int _maxHp;
    private int _attack;
    private int _defense;
    private int _speed;

    public string Name { get => _name; }
    public int Hp { get => _hp; set { _hp = value; StatChangeAction?.Invoke(this); } }
    public int MaxHp { get => _maxHp; set { _maxHp = value; StatChangeAction?.Invoke(this); } }
    public int Attack { get => _attack; set { _attack = value; StatChangeAction?.Invoke(this); } }
    public int Defense { get => _defense; set { _defense = value; StatChangeAction?.Invoke(this); } }

    public Action<CreatureStat> StatChangeAction;

    public virtual void SetStat(Data.CreatureData creatureData)
    {
        StatChangeAction = null;
        _name = creatureData.Name;
        _hp = creatureData.Hp;
        _maxHp = creatureData.Hp;
        _attack = creatureData.Attack;
        _defense = creatureData.Defense;
    }

    #region Event

    public void OnDamage(int damage, int attackCount = 1)
    {
        int trueDamage = Mathf.Max(damage - Defense, 1);
        if (attackCount > 1)
            trueDamage = Mathf.Max(trueDamage / attackCount, 1);

        Hp = Mathf.Clamp(Hp - trueDamage, 0, MaxHp);
    }

    public void OnHeal(int amount)
    {
        Hp = Mathf.Clamp(Hp + amount, 0, MaxHp);
    }
    
    #endregion
}
