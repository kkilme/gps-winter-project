using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// ������ ���� Ŭ����. �� �������鳢�� ���� ������.
public class UI_TownScene : UI_Scene
{
    private UI_Page _currentPage;

	enum GameObjects
	{
        Base, // �������� ����Ǿ �����ϴ� UI��
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

        GetButton(Buttons.Button_Quest).gameObject.BindEvent(OnClickedQuestButton, Define.UIEvent.Click);
        GetButton(Buttons.Button_ExitAtQuest).gameObject.BindEvent(OnClickedExitButton, Define.UIEvent.Click);
        GetButton(Buttons.Button_Store).gameObject.BindEvent(OnClickedStoreButton, Define.UIEvent.Click);
        GetButton(Buttons.Button_ExitAtStore).gameObject.BindEvent(OnClickedExitButton, Define.UIEvent.Click);

        Get<UI_Page>(Pages.UI_Page_Town_Main).gameObject.SetActive(true);
    }

    private void OnClickedQuestButton(PointerEventData data)
    {
        Get<UI_Page>(Pages.UI_Page_Town_Main).gameObject.SetActive(false);

        _currentPage = Get<UI_Page>(Pages.UI_Page_Town_Quest);
        _currentPage.gameObject.SetActive(true);
    }

    private void OnClickedStoreButton(PointerEventData data)
    {
        Get<UI_Page>(Pages.UI_Page_Town_Main).gameObject.SetActive(false);

        _currentPage = Get<UI_Page>(Pages.UI_Page_Town_Store);
        _currentPage.gameObject.SetActive(true);
    }

    private void OnClickedExitButton(PointerEventData data)
    {
        _currentPage.gameObject.SetActive(false);

        _currentPage = Get<UI_Page>(Pages.UI_Page_Town_Main);
        _currentPage.gameObject.SetActive(true);
    }
}
