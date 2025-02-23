using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 모든 UI의 조상 클래스
public abstract class UI_Base : MonoBehaviour
{
    protected Dictionary<Type, UnityEngine.Object[]> _objectDic = new Dictionary<Type, UnityEngine.Object[]>();
    
    public abstract void Init();

    private void Awake()
    {
        Init();
    }

    public void ShowInstantly()
    {
        gameObject.SetActive(true);
    }

    public void HideInstantly()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// DOTween을 사용해 UI를 보여주는 메소드
    /// </summary>
    public virtual Tween Show()
    {
        gameObject.SetActive(true);

        return DOVirtual.DelayedCall(0, () => {}); // 즉시 종료되는 Dummy Tween
    }

    /// <summary>
    /// DOTween을 사용해 UI를 숨기는 메소드
    /// </summary>
    public virtual Tween Hide()
    {
        gameObject.SetActive(false);

        return DOVirtual.DelayedCall(0, () => { });
    }

    // T컴포넌트를 가지고 있는 모든 자식 GameObject를 검색해 _objectDic에 Add
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] uiNames = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[uiNames.Length];
        _objectDic.Add(typeof(T), objects);

        for (int i = 0; i < uiNames.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = GlobalUtility.FindChild(gameObject, uiNames[i], true);
            else
                objects[i] = GlobalUtility.FindChild<T>(gameObject, uiNames[i], true);
            
            if (objects[i] == null)
                Debug.Log(($"Failed to bind({uiNames[i]})"));
        }
    }

    // T컴포넌트를 가지고 있으며 파라미터로 넘긴 idx에 해당하는 GameObject 검색 후 반환
    protected T Get<T>(Enum idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects;
        if (_objectDic.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[Convert.ToInt32(idx)] as T;
    }
    
    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects;
        if (_objectDic.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[idx] as T;
    }

    protected GameObject GetGameObject(Enum idx) { return Get<GameObject>(idx); }

    protected TextMeshProUGUI GetText(Enum idx) { return Get<TextMeshProUGUI>(idx); }

    protected Button GetButton(Enum idx) { return Get<Button>(idx); }

    protected Image GetImage(Enum idx) { return Get<Image>(idx); }

    // go가 이벤트 콜백(입력)을 받아 이벤트 함수를 실행할 수 있게 만든다.
    public static void BindEvent(GameObject go, Action<PointerEventData> action,
        UIEvent type = UIEvent.Click)
    {
        UI_EventHandler evt = GlobalUtility.GetOrAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case UIEvent.Drag:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;
            case UIEvent.Enter:
                evt.OnEnterHandler -= action;
                evt.OnEnterHandler += action;
                break;
            case UIEvent.Exit:
                evt.OnExitHandler -= action;
                evt.OnExitHandler += action;
                break;
            case UIEvent.Stay:
                evt.OnStayHandler -= action;
                evt.OnStayHandler += action;
                break;
            case UIEvent.DoubleClick:
                evt.OnDoubleClickHandler -= action;
                evt.OnDoubleClickHandler += action;
                break;
        }
    }

    public static void ClearEvent(GameObject go)
    {
        UI_EventHandler evt = GlobalUtility.GetOrAddComponent<UI_EventHandler>(go);

        evt.OnClickHandler = null;
        evt.OnDragHandler = null;
        evt.OnEnterHandler = null;
        evt.OnExitHandler = null;
        evt.OnStayHandler = null;
        evt.OnDoubleClickHandler = null;
    }
}
