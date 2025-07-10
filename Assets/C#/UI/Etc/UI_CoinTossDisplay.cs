using System.Collections;
using UnityEngine;

/// <summary>
/// 코인 토스를 보여주는 UI
/// </summary>
public class UI_CoinTossDisplay : UI_Base
{
    public enum Coins
    {
        Coin1,
        Coin2,
        Coin3,
        Coin4,
        Coin5,
        Coin6,
        Coin7,
        Coin8,
    }

    public override void Init()
    {
        Bind<UI_Coin>(typeof(Coins));
    }

    /// <summary>
    /// stat에 맞게 코인 이미지를 설정하고 coinCount만큼 활성화
    /// </summary>
    public void SetData(int coinCount, StatName stat)
    {   
        if(coinCount == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        for (int i = 0; i < GlobalValues.MAX_COIN_COUNT; i++)
        {
            if (i < coinCount)
                Get<UI_Coin>(i).LateInit(stat);
            else
                Get<UI_Coin>(i).gameObject.SetActive(false);
        }

        gameObject.SetActive(true);
    }

    public IEnumerator ShowResult(bool[] result, StatName stat)
    {
        SetData(result.Length, stat); // 몬스터 턴을 위해 필요
        for (int i = 0; i < result.Length; i++)
        {
            Get<UI_Coin>(i).ShowResult(result[i]);
            yield return new WaitForSeconds(0.3f);
        }
    }
}
