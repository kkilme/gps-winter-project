using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


// 퀘스트 1개 (QuestList UI의 한 줄)
public class UI_QuestList_Quest : UI_Base
{
    enum GameObjects
    {
        Detail_Unlocked,
        Detail_Locked
    }

    enum Buttons
    {
        Button_OpenDetail,
    }

    enum Texts
    {
        Text_ChapterStage,
        Text_QuestName,
    }

    public override void Init() {}

    public void LateInit(UI_QuestBoard questBoardUI, Quest quest)
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        if (!quest.QuestData.IsUnlocked)
        {
            // 퀘스트 미개방 시
            GetGameObject(GameObjects.Detail_Unlocked).SetActive(false);
            GetGameObject(GameObjects.Detail_Locked).SetActive(true);
        }
        else
        {
            // 퀘스트 개방 시
            GetGameObject(GameObjects.Detail_Unlocked).SetActive(true);
            GetGameObject(GameObjects.Detail_Locked).SetActive(false);
            GetText(Texts.Text_ChapterStage).text = quest.QuestData.Chapter + "-" + quest.QuestData.Stage;
            if (quest.QuestData.IsComplete) GetText(Texts.Text_ChapterStage).color = Color.green; // 한 번 완료한 퀘스트일 시
            GetText(Texts.Text_QuestName).text = quest.QuestData.Name;
            GetButton(Buttons.Button_OpenDetail).onClick.AddListener(() => questBoardUI.ShowQuestDetailPanel(quest));
        }
    }
}
