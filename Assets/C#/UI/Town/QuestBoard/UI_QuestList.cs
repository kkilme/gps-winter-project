using UnityEngine;
using UnityEngine.UI;

public class UI_QuestList : UI_Base
{
    enum Buttons
    {
        Button_Close
    }

    private UI_QuestBoard _questBoard;

    private ScrollRect _scrollRect;
    private Transform _contentTransform;

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        GetButton(Buttons.Button_Close).onClick.AddListener(Close);

        _scrollRect = GetComponentInChildren<ScrollRect>();
        _contentTransform = GetComponentInChildren<LayoutGroup>().gameObject.transform;
    }

    public void LateInit(UI_QuestBoard questBoard)
    {
        _questBoard = questBoard;
    }

    public new void Show()
    {
        // 기존 퀘스트 UI 파괴
        for (int i = _contentTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(_contentTransform.GetChild(i).gameObject);
        }

        // 퀘스트 목록 받아오기
        foreach (QuestData questData in Managers.DataMng.QuestDataDict.Values)
        {
            Quest quest = new Quest(questData);
            UI_QuestList_Quest questList_Quest = Managers.UIMng.MakeSubItemUI<UI_QuestList_Quest>(_contentTransform, "Town/" + nameof(UI_QuestList_Quest));
            questList_Quest.LateInit(_questBoard, quest);
        }
        _scrollRect.verticalNormalizedPosition = 1f; // 스크롤을 맨 위로 초기화
    }

    public void Close()
    {
        _questBoard.Close();
    }
}
