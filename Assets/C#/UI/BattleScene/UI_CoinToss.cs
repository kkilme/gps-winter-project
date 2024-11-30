using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class UI_CoinToss : UI_Base
{
    public enum Coins
    {
        Coin1,
        Coin2,
        Coin3,
        Coin4,
        Coin5
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(Coins));
    }
    
    public void InitTurn()
    {
        gameObject.SetActive(true);
    }

    public void EndTurn()
    {
        for (int i = 0; i < 5; i++)
        {
            Util.FindChild(Get<GameObject>(i), "SuccessIcon").SetActive(false);
            Util.FindChild(Get<GameObject>(i), "FailedIcon").SetActive(false);
        }
    }
    
    public void ShowCoinNum(BaseAction action)
    {
        for (int i = 0; i < 5; i++)
            Get<GameObject>(i).SetActive(i < action.CoinNum);
    }
    
    public void ShowCoinToss(BaseAction action, int coinHeadNum)
    {
        ShowCoinNum(action);

        for (int i = 0; i < 5; i++)
        {
            Util.FindChild(Get<GameObject>(i), "SuccessIcon").SetActive(i < coinHeadNum);
            Util.FindChild(Get<GameObject>(i), "FailedIcon").SetActive(i >= coinHeadNum);
        }
    }
}
