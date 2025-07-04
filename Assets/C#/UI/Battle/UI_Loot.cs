using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Loot : UI_Popup
{
    public Action<Loot> OnLootTakeComplete; // 전리품 획득 완료 시 호출되는 이벤트

    private RectTransform _actionRect;
    private RectTransform _itemDetailRect;
    private UI_ItemDetailPopup _itemDetailPopup;

    private Loot _lootDropped; // 나온 모든 전리품
    private Loot _lootTaken; // 플레이어가 획득하고자 선택한 전리품

    enum RectTransforms
    {
        UI_ItemDetailPopup,
        Panel_LootAction
    }

    enum Buttons
    {
        Button_Take,
        Button_Dispose
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<RectTransform>(typeof(RectTransforms));

        _actionRect = Get<RectTransform>(RectTransforms.Panel_LootAction);
        _actionRect.gameObject.SetActive(false);

        _itemDetailRect = Get<RectTransform>(RectTransforms.UI_ItemDetailPopup);
        _itemDetailPopup = Get<RectTransform>(RectTransforms.UI_ItemDetailPopup).GetComponent<UI_ItemDetailPopup>();

        _lootTaken = new Loot();
    }

    public void ShowGold(int goldAmount)
    {
        _itemDetailPopup.ApplyGoldDesign(goldAmount);

        _itemDetailRect.DOScale(new Vector3(0.2f, 0.2f, 1f), 0.5f).From().SetEase(Ease.InQuad).OnComplete(() =>
        {
            _actionRect.gameObject.SetActive(true);
        });

        GetButton(Buttons.Button_Take).onClick.AddListener(TakeLoot);
        GetButton(Buttons.Button_Dispose).onClick.AddListener(ShowNextLoot);

        void TakeLoot()
        {
            _lootTaken.Gold += goldAmount;
            ShowNextLoot();
        }
    }

    public void ShowItem(ItemData itemData)
    {
        _itemDetailPopup.ApplyDesign(itemData);

        _itemDetailRect.DOScale(new Vector3(0.2f, 0.2f, 1f), 0.5f).From().SetEase(Ease.InQuad).OnComplete(() =>
        {
            _actionRect.gameObject.SetActive(true);
        });

        GetButton(Buttons.Button_Take).onClick.AddListener(TakeLoot);
        GetButton(Buttons.Button_Dispose).onClick.AddListener(ShowNextLoot);

        void TakeLoot()
        {
            _lootTaken.Items.Add(itemData);
            ShowNextLoot();
        }
    }

    public void Show(Loot loot)
    {
        _lootDropped = loot;
        ShowNextLoot();
    }

    private int _currentLootIndex = -1;

    public void ShowNextLoot()
    {
        GetButton(Buttons.Button_Take).onClick.RemoveAllListeners();
        GetButton(Buttons.Button_Dispose).onClick.RemoveAllListeners();
        _actionRect.gameObject.SetActive(false);

        // 골드 전리품을 위한 코드
        if (_currentLootIndex == -1)
        {
            _currentLootIndex = 0;
            if (_lootDropped.Gold != 0)
            {
                // 골드가 0이 아니면 골드 전리품을 보여줌
                ShowGold(_lootDropped.Gold);
                return;
            }
            // 골드가 0이면 이후 코드 실행
        }

        // 전리품을 모두 보여줬을 경우(더 보여줄 전리품이 남아있지 않음)
        if(_currentLootIndex >= _lootDropped.Items.Count - 1)
        {
            _itemDetailRect.gameObject.SetActive(false);
            OnLootTakeComplete?.Invoke(_lootTaken);
            return;
        }

        // 현재 인덱스에 해당하는 아이템을 보여줌
        ItemData itemData = _lootDropped.Items[_currentLootIndex];
        ShowItem(itemData);
        _currentLootIndex++;
    }
}
