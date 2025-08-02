using UnityEngine;


public static class DamageCalculator
{
    public static int CalculateFinalDamage(Creature attacker, Creature opponent, AttackSkillData attackSkillData, int coinSuccessCount, int targetCount = 1)
    {
        var attackerStat = attacker.CreatureStat;
        var opponentStat = opponent.CreatureStat;

        var totalDamage = (attackerStat.BaseDamage / targetCount + attackSkillData.DamagePerCoin * coinSuccessCount); // 총 타겟 수만큼 나눔

        var defense = attackSkillData.AttackType == AttackType.Physical ? opponentStat.PhysicalDefense : opponentStat.MagicDefense;
        var finalDamage = Mathf.Max(0, totalDamage - defense);

        //Debug.Log($"[DamageCalculator] BaseDamage {attackerStat.BaseDamage} + CoinDamage {coinSuccessCount} * {attackSkillData.DamagePerCoin} - Defense {defense} = {finalDamage}");

        return finalDamage;
    }
}
