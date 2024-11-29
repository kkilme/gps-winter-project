using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private int _order = 10; // 현재까지 최근에 사용한 오더
    
    public UI_Scene SceneUI { get; protected set; } // 현재의 고정 캔버스 UI
    public Stack<UI_Popup> PopupStack { get; protected set; } // 팝업 캔버스 UI Stack

    public void Init()
    {
        PopupStack = new Stack<UI_Popup>();
    }
    
    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };

            return root;
        }
    }

    //sort가 true면, go의 Canvas 컴포넌트를 가져와 _order값을 1더해서 설정 (PopupUI)
    //sort가 false면, go의 Canvas 컴포넌트를 가져와 _order값을 0으로 설정(SceneUI)
    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        //canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true; // 부모 캔버스와는 독립적인 오더값을 가짐

        if (sort)
        {
            canvas.sortingOrder = _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    // 이름이 name인 SceneUI를 생성한 후 T컴포넌트로 반환
    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.ResourceMng.Instantiate($"UI/SceneUI/{name}");
        T sceneUI = Util.GetOrAddComponent<T>(go);
        SceneUI = sceneUI;

        go.transform.SetParent(Root.transform);
        
        return sceneUI;
    }

    // 이름이 name인 PopupUI를 생성한 후 T컴포넌트로 반환
    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.ResourceMng.Instantiate($"UI/PopupUI/{name}");
        T popupUI = Util.GetOrAddComponent<T>(go);
        PopupStack.Push(popupUI);
        
        go.transform.SetParent(Root.transform);
        
        return popupUI;
    }
    
    // T 타입의 UI 컴포넌트를 반환
    public T GetUIComponent<T>() where T: UI_Base
    {
        if (SceneUI == null)
        {
            Debug.Log("SceneUI is null!"); 
            return null;
        }
         
        T ui = Util.FindChild<T>(SceneUI.gameObject, recursive: true);
        return ui;
    }

    // 이름이 name인 SubItemUI를 생성한 후 T컴포넌트로 반환
    public T MakeSubItemUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.ResourceMng.Instantiate($"UI/SubItemUI/{name}");
        RectTransform   rectTransform = go.GetComponent<RectTransform>();
        if (parent != null)
        {
            // 부모 설정 후 스케일 변하는거 방지
            Vector3 localScale = rectTransform.localScale;
            go.transform.SetParent(parent);
            rectTransform.localScale = localScale;
        }
        
        return go.GetOrAddComponent<T>();
    }

    // 이름이 name인 WorldSpaceUI를 생성한 후 T컴포넌트로 반환
    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.ResourceMng.Instantiate($"UI/WorldSpaceUI/{name}");
        if (parent != null)
            go.transform.SetParent(parent);

        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return go.GetOrAddComponent<T>();
    }

    // 가장 Order가 높은 PopupUI 제거
    public void ClosePopupUI()
    {
        if (PopupStack.Count == 0)
            return;

        UI_Popup popupUI= PopupStack.Pop();
        Managers.ResourceMng.Destroy(popupUI.gameObject);
        popupUI = null;
        _order--;
    }

    // 가장 Order가 높은 PopupUI 확인 후 제거
    public void ClosePopupUI(UI_Popup popup)
    {
        if (PopupStack.Count == 0)
            return;

        if (PopupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed");
            return;
        }
        
        ClosePopupUI();
    }

    // 모든 PopupUI 제거
    public void CloseAllPopupUI()
    {
        while (PopupStack.Count > 0)
        {
            ClosePopupUI();
        }
    }
    
    public void Clear()
    {
        CloseAllPopupUI();
        SceneUI = null;
    }
}
