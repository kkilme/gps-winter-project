using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Loot : UI_Popup
{
    public Action<LootActionType> OnLootAction;

    private RectTransform _actionRect;
    private RectTransform _infoRect;

    private Item _loot;
    private bool _isGold;
    private uint _quantity;

    enum RectTransforms
    {
        Panel_LootInfo,
        Panel_LootAction
    }

    enum Images
    {
        LootImage,
        LootTypeIcon,
    }

	enum Texts
	{
		Text_LootName,
        Text_LootQuantity,
        Text_LootDescription,
	}

    enum Buttons
    {
        Button_Take,
        Button_Dispose
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<RectTransform>(typeof(RectTransforms));
        Bind<Image>(typeof(Images));

        _actionRect = Get<RectTransform>(RectTransforms.Panel_LootAction);
        _actionRect.gameObject.SetActive(false);

        _infoRect = Get<RectTransform>(RectTransforms.Panel_LootInfo);

        GetButton(Buttons.Button_Take).onClick.AddListener(TakeReward);
        GetButton(Buttons.Button_Dispose).onClick.AddListener(() =>
        {
            OnLootAction.Invoke(LootActionType.Dispose);
        });
    }

    // 골드 전리품일 시
    public void Init(uint goldAmount)
    {
        _quantity = goldAmount;
        _isGold = true;
        GetImage(Images.LootImage).sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.ITEMIMAGE_PATH_PREFIX + "Gold");
        GetImage(Images.LootTypeIcon).sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.ITEMICON_PATH_PREFIX + "Gold");
        GetText(Texts.Text_LootDescription).text = "Nobody hates Gold, right?";
        GetText(Texts.Text_LootName).text = "Gold";
        GetText(Texts.Text_LootQuantity).text = $"x {_quantity}";
    }

    public void Init(Item item)
    {
        _loot = item;
        _quantity = 1;
        _isGold = false;
        // TODO: 아이템 데이터에 맞게 이미지 및 텍스트 적용
    }

    private void TakeReward()
    {
        if (_isGold)
        {
            Managers.AreaMng.Loots.Gold += _quantity;
        }
        else
        {
            // TODO: 아이템 지급
        }

        OnLootAction.Invoke(LootActionType.Take);
    }

    public override Tween Show()
    {
        return _infoRect.DOScale(new Vector3(0.2f, 0.2f, 1f), 0.5f).From().SetEase(Ease.InQuad).OnComplete(() =>
        {
            _actionRect.gameObject.SetActive(true);
        });
    }
}
