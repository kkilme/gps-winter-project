using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UI_BattleVictory : UI_Base
{
    // TODO: Test code
    private Dictionary<int, int> testrewards = new() { { 1, 2 }, { 2, 1 }, { 3, 1 } };

    enum RectTransforms
    {
        VictoryTitle,
        VictoryDescription
    }

    enum Text
    {
        Text_VictoryDescription
    }

    public override void Init()
    {
        Bind<RectTransform>(typeof(RectTransforms));
        Bind<TextMeshProUGUI>(typeof(Text));
    }

    private void OnEnable()
    {
        StartCoroutine(UIAnimation());
    }

    private IEnumerator UIAnimation()
    {
        RectTransform titleRect = Get<RectTransform>(RectTransforms.VictoryTitle);
        RectTransform descRect = Get<RectTransform>(RectTransforms.VictoryDescription);

        Sequence seq = DOTween.Sequence();

        seq.Append(titleRect.DOScale(new Vector3(.2f, .2f, 1f), .3f).From().SetEase(Ease.InQuad));
        seq.AppendInterval(1f);
        seq.Append(titleRect.DOScale(new Vector3(1f, 1f, 1f), 1f));
        seq.Join(titleRect.DOAnchorPos(new Vector2(-680f, 440f), 1f).OnComplete(() =>
        {
            Vector3 temp = titleRect.transform.position;

            titleRect.anchorMax = new Vector2(0, 1);
            titleRect.anchorMin = new Vector2(0, 1);

            titleRect.transform.position = temp;
        }));
        seq.Append(descRect.DOAnchorPosX(240, 1.5f).SetEase(Ease.OutCirc));

        seq.Play();
        yield return seq.WaitForCompletion();
        
        ShowReward();

    }

    private void ShowReward(int recentReward = -1)
    {
        UI_Reward rewardUI = Managers.UIMng.ShowPopupUI<UI_Reward>();

        KeyValuePair<int, int> reward = testrewards.Last();
        int rewardId = reward.Key;
        int rewardQuantity = reward.Value;

        // Pass 선택 시 RewardUI 애니메이션을 다시 재생하지 않도록 하기 위한 코드
        if (recentReward == rewardId)
        {
            rewardUI.Init(rewardId, rewardQuantity, false);
        }
        else
        {
            rewardUI.Init(rewardId, rewardQuantity);
        }

        testrewards.Remove(rewardId);

        Debug.Log($"Current reward: {rewardId}");

        void OnRewardAction(Define.RewardActionType action)
        {   
            Managers.UIMng.ClosePopupUI(rewardUI);
            switch (action)
            {
                case Define.RewardActionType.Take:
                case Define.RewardActionType.Dispose:
                    if (testrewards.Count == 0)
                    {
                        // TODO: BattleScene 언로딩 (AreaScene 복귀)
                    }
                    else
                    {
                        ShowReward(rewardId);
                    }
                    break;
                case Define.RewardActionType.Pass:
                    testrewards.Add(rewardId, rewardQuantity);
                    ShowReward(rewardId);
                    break;
            }

        }

        rewardUI.OnRewardAction -= OnRewardAction;
        rewardUI.OnRewardAction += OnRewardAction;

    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
    