using UnityEngine;

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
}
