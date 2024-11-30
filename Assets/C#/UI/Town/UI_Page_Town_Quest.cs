using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Page_Town_Quest : UI_Page
{
    enum Buttons
    {
        Button_QuestBoard,
    }

    enum GameObjects
    {
        QuestBoard,
        UI_Quest,
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        void OnClickedQuestBoard()
        {
            GetGameObject(GameObjects.QuestBoard).SetActive(true);
        }

        GetButton(Buttons.Button_QuestBoard).onClick.AddListener(OnClickedQuestBoard);
    }

    private void OnDisable()
    {
        GetGameObject(GameObjects.QuestBoard).SetActive(false);
        GetGameObject(GameObjects.UI_Quest).SetActive(false);
    }
}
