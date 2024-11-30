using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Reward : UI_Popup
{
    public Action<Define.RewardActionType> OnRewardAction;

    private RectTransform _actionRect;
    private RectTransform _infoRect;

    private int _reward;
    private int _quantity;
    enum RectTransforms
    {
        Panel_RewardInfo,
        Panel_RewardAction
    }

    enum Images
    {
        RewardImage,
        RewardTypeIcon,
    }

	enum Texts
	{
		Text_RewardName,
        Text_ActionOwner,
        Text_RewardQuantity
	}

    enum Buttons
    {
        Button_Take,
        Button_Pass,
        Button_Dispose
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<RectTransform>(typeof(RectTransforms));
        Bind<Image>(typeof(Images));

        _actionRect = Get<RectTransform>(RectTransforms.Panel_RewardAction);
        _actionRect.gameObject.SetActive(false);

        _infoRect = Get<RectTransform>(RectTransforms.Panel_RewardInfo);

        GetButton(Buttons.Button_Take).onClick.AddListener(TakeReward);
        GetButton(Buttons.Button_Pass).onClick.AddListener( () =>
        {
            OnRewardAction.Invoke(Define.RewardActionType.Pass);
        });
        GetButton(Buttons.Button_Dispose).onClick.AddListener(() =>
        {
            OnRewardAction.Invoke(Define.RewardActionType.Dispose);
        });


    }

    public void Init(int dataId, int quantity = 1, bool doAnimation = true)
    {
        _reward = dataId;
        _quantity = quantity;
        // TODO: 아이템 데이터에 맞게 이미지 및 텍스트 적용

        GetText(Texts.Text_RewardQuantity).text = _quantity != 1 ? $"x {quantity}" : "";
        
        if (doAnimation)
        {
            StartCoroutine(UIAnimation());
        }
        else
        {
            _actionRect.gameObject.SetActive(true);
        }
    }

    private void TakeReward()
    {
        // TODO: 현재 행동한 플레이어에게 아이템 지급

        OnRewardAction.Invoke(Define.RewardActionType.Take);
    }
    
    private IEnumerator UIAnimation()
    {
        yield return _infoRect.DOScale(new Vector3(0.2f, 0.2f, 1f), 0.5f).From().SetEase(Ease.InQuad).WaitForCompletion();
        yield return new WaitForSeconds(0.5f);

        _actionRect.gameObject.SetActive(true);
    }
}
