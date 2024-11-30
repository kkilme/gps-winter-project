using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 페이지 관리 클래스. 각 페이지들끼리 연결 시켜줌.
public class UI_TownScene : UI_Scene
{
    private UI_Page _currentPage;

	enum GameObjects
	{
        Base, // 페이지가 변경되어도 존재하는 UI들
    }

    enum Pages
    {
        UI_Page_Town_Main,
        UI_Page_Town_Quest,
        UI_Page_Town_Store,
    }

    enum Buttons
    {
        Button_Quest,
        Button_ExitAtQuest,
        Button_Store,
        Button_ExitAtStore,
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<UI_Page>(typeof(Pages));
        Bind<Button>(typeof(Buttons));

        void OnClickedQuestButton(PointerEventData data)
        {
            Get<UI_Page>(Pages.UI_Page_Town_Main).gameObject.SetActive(false);

            _currentPage = Get<UI_Page>(Pages.UI_Page_Town_Quest);
            _currentPage.gameObject.SetActive(true);
        }

        void OnClickedStoreButton(PointerEventData data)
        {
            Get<UI_Page>(Pages.UI_Page_Town_Main).gameObject.SetActive(false);

            _currentPage = Get<UI_Page>(Pages.UI_Page_Town_Store);
            _currentPage.gameObject.SetActive(true);
        }

        void OnClickedExitButton(PointerEventData data)
        {
            _currentPage.gameObject.SetActive(false);

            _currentPage = Get<UI_Page>(Pages.UI_Page_Town_Main);
            _currentPage.gameObject.SetActive(true);
        }

        GetButton(Buttons.Button_Quest).gameObject.BindEvent(OnClickedQuestButton, Define.UIEvent.Click);
        GetButton(Buttons.Button_ExitAtQuest).gameObject.BindEvent(OnClickedExitButton, Define.UIEvent.Click);
        GetButton(Buttons.Button_Store).gameObject.BindEvent(OnClickedStoreButton, Define.UIEvent.Click);
        GetButton(Buttons.Button_ExitAtStore).gameObject.BindEvent(OnClickedExitButton, Define.UIEvent.Click);

        Get<UI_Page>(Pages.UI_Page_Town_Main).gameObject.SetActive(true);
    }
}
