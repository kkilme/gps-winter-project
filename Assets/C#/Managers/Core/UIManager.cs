using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// UI의 생성 및 제거 역할 담당
/// </summary>
public class UIManager
{
    private int _order = 10; // 현재까지 최근에 사용한 오더
    private GameObject _root;
    
    public UI_Scene SceneUI { get; protected set; } // 현재의 고정 캔버스 UI
    public List<UI_Popup> PopupUIs { get; protected set; }

    public void Init()
    {
        PopupUIs = new List<UI_Popup>();
    }
    
    public GameObject Root
    {
        get
        {
            if (_root == null)
            {
                _root = GameObject.Find("@UI_Root");
                if (_root == null)
                    _root = new GameObject { name = "@UI_Root" };
            }
            return _root;
        }
    }

    //sort가 true면, go의 Canvas 컴포넌트를 가져와 _order값을 1더해서 설정 (PopupUI)
    //sort가 false면, go의 Canvas 컴포넌트를 가져와 _order값을 0으로 설정(SceneUI)
    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = GlobalUtility.GetOrAddComponent<Canvas>(go);
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

    private void SetUIParent(GameObject ui, Transform parent, bool forceRootParent = true)
    {
        if (parent != null)
        {
            var rectTransform = ui.GetComponent<RectTransform>();
            Vector3 localScale = rectTransform.localScale;
            ui.transform.SetParent(parent);
            rectTransform.localScale = localScale;
        }
        else if(forceRootParent)
        {
            ui.transform.SetParent(Root.transform);
        }
    }

    /// <summary>
    /// UI/SceneUI/{path}의 SceneUI를 생성한 후 T컴포넌트로 반환
    /// </summary>
    public T ShowSceneUI<T>(string path = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(path))
            path = typeof(T).Name;

        GameObject go = Managers.ResourceMng.Instantiate($"UI/SceneUI/{path}");
        T sceneUI = GlobalUtility.GetOrAddComponent<T>(go);
        SceneUI = sceneUI;

        SetUIParent(go, Root.transform);

        return sceneUI;
    }

    /// <summary>
    /// UI/GeneralUI/{path}의 UI를 생성한 후 T컴포넌트로 반환
    /// </summary>
    public T MakeGeneralUI<T>(Transform parent = null, string path = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(path))
            path = typeof(T).Name;

        GameObject go = Managers.ResourceMng.Instantiate($"UI/GeneralUI/{path}");
        
        SetUIParent(go, parent, false);

        return go.GetOrAddComponent<T>();
    }

    /// <summary>
    /// UI/SubItemUI/{path}의 SubItemUI를 생성한 후 T컴포넌트로 반환
    /// </summary>
    public T MakeSubItemUI<T>(Transform parent = null, string path = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(path))
            path = typeof(T).Name;

        GameObject go = Managers.ResourceMng.Instantiate($"UI/SubItemUI/{path}");
        
        SetUIParent(go, parent);

        return go.GetOrAddComponent<T>();
    }

    /// <summary>
    /// UI/WorldSpaceUI/{path}의 WorldSpaceUI를 생성한 후 T컴포넌트로 반환
    /// </summary>
    public T MakeWorldSpaceUI<T>(Transform parent = null, string path = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(path))
            path = typeof(T).Name;

        GameObject go = Managers.ResourceMng.Instantiate($"UI/WorldSpaceUI/{path}");

        SetUIParent(go, parent);

        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return go.GetOrAddComponent<T>();
    }

    /// <summary>
    /// UI/PopupUI/{path}의 PopupUI를 생성한 후 T컴포넌트로 반환
    /// </summary>
    public T ShowPopupUI<T>(string path = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(path))
            path = typeof(T).Name;

        GameObject go = Managers.ResourceMng.Instantiate($"UI/PopupUI/{path}");
        T popupUI = GlobalUtility.GetOrAddComponent<T>(go);

        Canvas canvas = GlobalUtility.GetOrAddComponent<Canvas>(go);
        canvas.sortingOrder = _order++;
        PopupUIs.Add(popupUI);
        
        SetUIParent(go, Root.transform);
        
        return popupUI;
    }

    /// <summary>
    /// 가장 최근에 열린 PopupUI 제거
    /// </summary>
    public void CloseTopPopupUI()
    {
        if (PopupUIs.Count == 0)
            return;

        UI_Popup popupUI= PopupUIs[^1];
        Managers.ResourceMng.Destroy(popupUI.gameObject);
        PopupUIs.RemoveAt(PopupUIs.Count - 1);

        _order = PopupUIs.Count == 0 ? 10 : Mathf.Max(10, _order - 1);
    }

    /// <summary>
    /// T타입 PopupUI를 찾아 존재하면 제거
    /// </summary>
    public void ClosePopupUI<T>() where T : UI_Popup
    {
        var popup = PopupUIs.FirstOrDefault(p => p is T);
        popup?.Close();
    }

    /// <summary>
    /// 특정 PopupUI를 제거
    /// </summary>
    public void ClosePopupUI(UI_Popup popup)
    {
        if (PopupUIs.Contains(popup))
        {
            if (PopupUIs[^1] == popup) CloseTopPopupUI();
            else
            {
                PopupUIs.Remove(popup);
                Managers.ResourceMng.Destroy(popup.gameObject);
            }
        }
    }

    /// <summary>
    /// 모든 PopupUI 제거
    /// </summary>
    public void CloseAllPopupUI()
    {
        while (PopupUIs.Count > 0)
        {
            CloseTopPopupUI();
        }
    }

    /// <summary>
    /// T타입 PopupUI를 열거나 닫음 (Toggle)
    /// </summary>
    public void TogglePopupUI<T>() where T : UI_Popup
    {
        var existingPopup = PopupUIs.FirstOrDefault(p => p is T);
        if (existingPopup != null)
        {
            ClosePopupUI<T>();
        }
        else
        {
            ShowPopupUI<T>();
        }
    }

    public void Clear()
    {
        CloseAllPopupUI();
        SceneUI = null;
    }
}
