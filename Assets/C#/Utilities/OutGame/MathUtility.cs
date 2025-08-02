public static class MathUtility
{
    /// <summary>
    /// 이항계수 계산 함수 (nCk)
    /// </summary>
    public static double BinomialCoefficient(int n, int k)
    {
        if (k < 0 || k > n) return 0;
        if (k == 0 || k == n) return 1;

        // 이항계수 계산
        // nCk = n! / ((n-k)! * k!) = n * (n-1) * ... * (n-k+1) / k!
        double result = 1;
        for (int i = 1; i <= k; i++)
        {
            result *= (n - (k - i));
            result /= i;
        }

        return result;
    }
}
