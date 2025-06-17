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
    private UI_Inventory _itemBag;
    private Transform _rewardContentTransform;

    public override void Init() {}

    public void LateInit(UI_QuestBoard questBoard)
    {
        ShowInstantly();
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<GridLayoutGroup>(typeof(GridLayoutGroups));

        _itemBag = Get<GridLayoutGroup>(GridLayoutGroups.ItemBag).GetComponent<UI_Inventory>();
        _rewardContentTransform = Get<GridLayoutGroup>(GridLayoutGroups.Contents_Rewards).GetComponent<Transform>();

        GetButton(Buttons.Button_Close).onClick.AddListener(Close);
        _questBoard = questBoard;
        HideInstantly();
    }

    public void Show(Quest quest)
    {
        ShowInstantly();
        QuestData data = quest.QuestData;
        GetText(Texts.Text_QuestName).text = data.Name;
        GetText(Texts.Text_Objective).text = data.Objective;
        GetGameObject(GameObjects.Flag_RewardClaimed).gameObject.SetActive(data.IsComplete);
        GetButton(Buttons.Button_Start).onClick.RemoveAllListeners();
        GetButton(Buttons.Button_Start).onClick.AddListener(()=>StartQuest(quest));

    }

    private void StartQuest(Quest quest)
    {
        this.gameObject.SetActive(false);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
