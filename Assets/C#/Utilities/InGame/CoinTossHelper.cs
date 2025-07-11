using UnityEngine;
using System;
using Random = UnityEngine.Random;

public static class CoinTossHelper
{
    public struct CoinTossResult
    {
        public bool[] result;
        public int successCount;
        public int failCount;
        public CoinTossResult(int coinCount)
        {
            result = new bool[coinCount];
            successCount = 0;
            failCount = 0;
        }
    }

    /// <summary>
    /// coinCount개수만큼의 동전을 각각 chance%로 던진 결과를 반환
    /// </summary>
    /// <param name="coinCount">던질 동전 개수</param>
    /// <param name="chance">동전이 앞면이 나올 확률(성공할 확률), 단위 %</param>
    public static CoinTossResult CoinToss(int coinCount, int chance)
    {
        CoinTossResult result = new CoinTossResult(coinCount);

        for (int i = 0; i < coinCount; i++)
        {
            bool isSuccess = Random.value < chance / 100f;

            result.result[i] = isSuccess;
            if (isSuccess) result.successCount++;
            else result.failCount++;
        }

        return result;
    }

    /// <summary>
    /// 코인별 성공확률이 successChancePerCoin(%)일 때, totalCoinCount만큼의 코인 중 정확히 coinCountRequired개가 성공할 확률(0-1)을 계산 및 반환. 
    /// </summary>
    /// <param name="successChancePerCoin">코인 하나당 성공 확률(%단위)</param>
    public static double CalculateChanceOfCoinTossResult(int totalCoinCount, int coinCountRequired, int successChancePerCoin)
    {
        double p = successChancePerCoin / 100.0;
        double q = 1.0 - p;

        // 이항 계수 nCk 계산
        double bc = MathUtility.BinomialCoefficient(totalCoinCount, coinCountRequired);

        // 확률 계산 - 이항 분포
        double probability = bc * Math.Pow(p, coinCountRequired) * Math.Pow(q, totalCoinCount - coinCountRequired);

        return probability;
    }

    /// <summary>
    /// 코인별 성공확률이 successChancePerCoin(%)일 때, totalCoinCount만큼의 코인 중 minCoinCountRequired ~ maxCoinCountRequired개가 성공할 확률(0-1)을 계산 및 반환.
    /// </summary>
    public static double CalculateChanceOfCoinTossResult(int totalCoinCount, int minCoinCountRequired, int maxCoinCountRequired, int successChancePerCoin)
    {
        double totalChance = 0;
        for (int i = minCoinCountRequired; i <= maxCoinCountRequired; i++)
        {
            totalChance += CalculateChanceOfCoinTossResult(totalCoinCount, i, successChancePerCoin);
        }

        return totalChance;
    }

    /// <summary>
    /// 코인별 성공확률이 successChancePerCoin(%)일 때, totalCoinCount만큼의 코인 중 정확히 coinCountRequired개가 성공할 확률(0%-100%)을 계산 및 반환.
    /// </summary>
    public static int CalculateChanceOfCoinTossResultAsPercent(int totalCoinCount, int coinCountRequired, int successChancePerCoin)
    {
        double result = CalculateChanceOfCoinTossResult(totalCoinCount, coinCountRequired, successChancePerCoin);

        return (int)Math.Round(result * 100);
    }

    /// <summary>
    /// 코인별 성공확률이 successChancePerCoin(%)일 때, totalCoinCount만큼의 코인 중 minCoinCountRequired ~ maxCoinCountRequired개가 성공할 확률(0%-100%)을 계산 및 반환. 
    /// </summary>
    public static int CalculateChanceOfCoinTossResultAsPercent(int totalCoinCount, int minCoinCountRequired, int maxCoinCountRequired, int successChancePerCoin)
    {
        double result = CalculateChanceOfCoinTossResult(totalCoinCount, minCoinCountRequired, maxCoinCountRequired, successChancePerCoin);

        return (int)Math.Round(result * 100);
    }

}
