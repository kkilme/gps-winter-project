using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


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
        ItemInventory.LateInit(onSlotClickAction: OnItemInventorySlotClicked, slotDesign: new PlusIconItemSlotDesign(), maxSize: 8);
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
        foreach(QuestReward reward in data.Rewards)
        {
            int rewardDataId = reward.ItemDataId;
            ItemData itemData = Managers.DataMng.ItemDataDict[rewardDataId];
            _rewardInventory.AddItem(itemData, reward.Quantity);
        }

        ItemInventory.Clear();
        GetText(Texts.Text_ItemCount).text = $"<color=#F8913F>0</color> / 8";
    }

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

    public void AddItem(ItemData itemData)
    {
        ItemInventory.AddItem(itemData);
        int notEmptySlotCnt = ItemInventory.InventorySlots.FindAll(x => !x.IsEmpty).Count;
        GetText(Texts.Text_ItemCount).text = $"<color=#F8913F>{notEmptySlotCnt}</color> / 8";
    }

    private void StartQuest(Quest quest)
    {
        
    }

    public void Close()
    {
        Managers.UIMng.ClosePopupUI<UI_QuestItemSelectPopup>();
        gameObject.SetActive(false);
    }
}
