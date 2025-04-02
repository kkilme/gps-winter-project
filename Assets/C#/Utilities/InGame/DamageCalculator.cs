using System.Collections;
using UnityEngine;


public static class DamageCalculator
{
    public static int CalculateFinalDamage(Creature attacker, Creature opponent, AttackSkillData attackSkillData, int coinSuccessCount, int targetCount = 1)
    {
        var attackerStat = attacker.CreatureStat;
        var opponentStat = opponent.CreatureStat;

        var baseDamage = (attackerStat.BaseDamage / targetCount) + attackSkillData.DamagePerCoin * coinSuccessCount;

        var defense = attackSkillData.AttackType == AttackType.Physical ? opponentStat.PhysicalDefense : opponentStat.MagicDefense;
        var finalDamage = Mathf.Max(0, baseDamage - defense);

        //Debug.Log($"[DamageCalculator] BaseDamage {attackerStat.BaseDamage} + CoinDamage {coinSuccessCount} * {attackSkillData.DamagePerCoin} - Defense {defense} = {finalDamage}");

        return finalDamage;
    }
}
