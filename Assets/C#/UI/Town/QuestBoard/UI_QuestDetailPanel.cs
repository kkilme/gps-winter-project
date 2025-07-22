using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// 선택한 퀘스트의 상세 정보를 보여주는 UI 패널.
/// </summary>
public class UI_QuestDetailPanel : UI_Base
{
    enum Buttons
    {
        Button_Close,
        Button_Start,
    }

    enum Texts
    {
        Text_QuestName,
        Text_Objective,

        Text_ItemCount
    }

    enum GameObjects
    {
        Flag_RewardClaimed,
    }

    enum GridLayoutGroups
    {
        RewardInventory,
        ItemInventory,
    }

    public UI_Inventory ItemInventory; // Area에 가져갈 아이템을 보여주는 인벤토리
    private UI_QuestItemSelectPopup _questItemSelectPopup;

    private UI_Inventory _rewardInventory; // 퀘스트 첫 클리어 보상을 보여주는 인벤토리

    public override void Init() {}

    public void LateInit()
    {
        ShowInstantly();
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<GridLayoutGroup>(typeof(GridLayoutGroups));

        ItemInventory = Get<GridLayoutGroup>(GridLayoutGroups.ItemInventory).GetComponent<UI_Inventory>();
        ItemInventory.LateInit(onSlotClickAction: OnItemInventorySlotClicked, slotDesign: new PlusIconItemSlotDesign(), maxSize: GlobalValues.MAX_AREAITEM_COUNT);
        _rewardInventory = Get<GridLayoutGroup>(GridLayoutGroups.RewardInventory).GetComponent<UI_Inventory>();
        _rewardInventory.LateInit(onSlotClickAction: null, slotDesign: new QuestRewardSlotDesign());

        GetButton(Buttons.Button_Close).onClick.AddListener(Close);
        HideInstantly();
    }

    /// <summary>
    /// 퀘스트를 바인딩하여 UI에 정보 반영
    /// </summary>
    public void BindQuest(Quest quest)
    {
        ShowInstantly();

        QuestData data = quest.QuestData;

        GetText(Texts.Text_QuestName).text = data.Name;
        GetText(Texts.Text_Objective).text = data.Objective;
        GetGameObject(GameObjects.Flag_RewardClaimed).SetActive(data.IsComplete);
        GetButton(Buttons.Button_Start).onClick.RemoveAllListeners();
        GetButton(Buttons.Button_Start).onClick.AddListener(()=>StartQuest(quest));

        _rewardInventory.HardClear();
        foreach(QuestReward reward in data.FirstClearRewards)
        {
            int rewardDataId = reward.ItemDataId;
            ItemData itemData = Managers.DataMng.ItemDataDict[rewardDataId];
            _rewardInventory.AddItem(itemData, reward.Quantity);
        }

        ItemInventory.Clear(false);
        GetText(Texts.Text_ItemCount).text = $"<color=#F8913F>0</color> / 8";
    }

    /// <summary>
    /// ItemInventory의 슬롯을 클릭했을 때 호출되는 콜백 함수.
    /// </summary>
    private void OnItemInventorySlotClicked(UI_ItemSlot selectedSlot)
    {
        if (selectedSlot.IsEmpty)
        {
            // 빈 슬롯 클릭 시, 아이템 선택 팝업창을 띄움.
            // 이미 팝업창이 존재할 시 새로 띄우지 않음.
            if (_questItemSelectPopup != null) return;
            UI_QuestItemSelectPopup popup = Managers.UIMng.ShowPopupUI<UI_QuestItemSelectPopup>();
            popup.LateInit(this);
            _questItemSelectPopup = popup;
        } 
        else
        {
            // 이미 아이템이 담긴 슬롯 클릭 시, 아이템을 제거함
            if (_questItemSelectPopup != null)
            {
                _questItemSelectPopup.PopupInventory.AddItem(selectedSlot.ItemData, enableStack: true); // 아이템 제거 전, 팝업창에 다시 아이템 추가
            }

            ItemInventory.UnbindSlot(selectedSlot);
            int notEmptySlotCnt = ItemInventory.InventorySlots.FindAll(x => !x.IsEmpty).Count;
            GetText(Texts.Text_ItemCount).text = $"<color=#F8913F>{notEmptySlotCnt}</color> / 8";
        }
    }

    /// <summary>
    /// ItemInventory에 itemData를 추가하는 메서드.
    /// </summary>
    public void AddItem(ItemData itemData)
    {
        Managers.SoundMng.PlayItemEffect(itemData.SoundPath, .4f);
        ItemInventory.AddItem(itemData);
        int notEmptySlotCnt = ItemInventory.InventorySlots.FindAll(x => !x.IsEmpty).Count;
        GetText(Texts.Text_ItemCount).text = $"<color=#F8913F>{notEmptySlotCnt}</color> / 8";
    }

    /// <summary>
    /// 선택된 퀘스트를 시작하는 메서드. AreaScene을 로드하는 메소드를 호출한다.
    /// </summary>
    private void StartQuest(Quest quest)
    {
        if (quest == null || !quest.QuestData.IsUnlocked)
        {
            Debug.LogError("[UI_QuestDetailPanel] Attempted to start an invalid or locked quest.");
            return;
        }

        List<ItemData> itemDatas = ItemInventory.InventorySlots
            .FindAll(slot => !slot.IsEmpty)
            .ConvertAll(slot => slot.ItemData);

        AreaInitContext areaInitContext = new(quest, itemDatas);
        foreach (ItemData item in areaInitContext.Items)
        {
            Managers.InvMng.RemoveItemByItemDataId(item.DataId, 1); // 선택한 아이템을 인벤토리에서 제거
        }

        Managers.SoundMng.PlayEffect("quest_start", .2f);
        CoroutineRunner.Instance.StartCoroutine(Managers.SceneMng.LoadAreaScene(areaInitContext)); // Area 씬 로드 시작
    }

    public void Close()
    {
        Managers.UIMng.ClosePopupUI<UI_QuestItemSelectPopup>();
        gameObject.SetActive(false);
    }
}
