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
        gameObject.SetActive(false);
    }
    
    public void InitTurn()
    {
        gameObject.SetActive(true);
    }

    public void EndTurn()
    {
        for (int i = 0; i < 5; i++)
        {
            GlobalUtility.FindChild(Get<GameObject>(i), "SuccessIcon").SetActive(false);
            GlobalUtility.FindChild(Get<GameObject>(i), "FailedIcon").SetActive(false);
        }
    }
    
    public void ShowCoinNum(BaseSkill skill)
    {
        for (int i = 0; i < 5; i++)
            Get<GameObject>(i).SetActive(i < skill.SkillData.CoinCount);
    }
    
    public void ShowCoinToss(BaseSkill skill, int coinHeadNum)
    {
        ShowCoinNum(skill);

        for (int i = 0; i < 5; i++)
        {
            GlobalUtility.FindChild(Get<GameObject>(i), "SuccessIcon").SetActive(i < coinHeadNum);
            GlobalUtility.FindChild(Get<GameObject>(i), "FailedIcon").SetActive(i >= coinHeadNum);
        }
    }
}
