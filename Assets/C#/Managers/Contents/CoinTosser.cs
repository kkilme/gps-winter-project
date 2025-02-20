using Data;
using System.Collections;
using UnityEngine;

public static class CoinTossser
{
    public static int CoinToss(StatName usingStat, CreatureStat stat, SkillData actionData)
    {
        if (usingStat == StatName.None)
            return -1;

        int coinHeadCount = 0;
        int statValue = stat.NameToStat(usingStat);
        for (int i = 0; i < actionData.CoinCount; i++)
        {
            float value = Random.value;
            if (value < statValue / 100f)
                coinHeadCount++;
        }


        return coinHeadCount;
    }
}
