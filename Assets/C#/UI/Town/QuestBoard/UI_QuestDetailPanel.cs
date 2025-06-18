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
        Contents_Rewards,
        ItemBag,
    }

    private UI_QuestBoard _questBoard;
    private UI_Inventory _itemInventory; // Area에 가져 갈 아이템을 보여주는 인벤토리
    private UI_Inventory _rewardInventory; // 퀘스트 첫 클리어 보상을 보여주는 인벤토리

    public override void Init() {}

    public void LateInit(UI_QuestBoard questBoard)
    {
        ShowInstantly();
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<GridLayoutGroup>(typeof(GridLayoutGroups));

        _itemInventory = Get<GridLayoutGroup>(GridLayoutGroups.ItemBag).GetComponent<UI_Inventory>();
        _itemInventory.LateInit(); // todo
        _rewardInventory = Get<GridLayoutGroup>(GridLayoutGroups.Contents_Rewards).GetComponent<UI_Inventory>();
        _rewardInventory.LateInit(onSlotClickAction: null, slotDesign: new QuestRewardSlotDesign());

        GetButton(Buttons.Button_Close).onClick.AddListener(Close);
        _questBoard = questBoard;
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

    }

    private void StartQuest(Quest quest)
    {
        
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
