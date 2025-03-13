using System.Collections;
using UnityEngine;

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

    public void ShowDeafult(int coinCount, StatName stat)
    {   
        if(coinCount == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        for (int i = 0; i < GlobalValues.MAX_COINT_COUNT; i++)
        {
            if (i < coinCount)
                Get<UI_Coin>(i).ShowDefault(stat);
            else
                Get<UI_Coin>(i).gameObject.SetActive(false);
        }

        gameObject.SetActive(true);
    }

    public IEnumerator ShowResult(bool[] result, StatName stat)
    {
        ShowDeafult(result.Length, stat); // 몬스터 턴을 위해 필요
        for (int i = 0; i < result.Length; i++)
        {
            Get<UI_Coin>(i).ShowResult(result[i]);
            yield return new WaitForSeconds(0.3f);
        }
    }
}
