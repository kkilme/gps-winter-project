using DG.Tweening;
using System;
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

    public virtual void ShowInstantly()
    {
        gameObject.SetActive(true);
    }

    public virtual void HideInstantly()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// DOTween을 사용해 UI를 보여주는 메소드. 필요 시 오버라이드하여 구현.
    /// </summary>
    public virtual Tween Show()
    {
        gameObject.SetActive(true);

        return DOVirtual.DelayedCall(0, () => { }); // 즉시 종료되는 Dummy Tween
    }

    /// <summary>
    /// DOTween을 사용해 UI를 숨기는 메소드. 필요 시 오버라이드하여 구현.
    /// </summary>
    public virtual Tween Hide()
    {
        gameObject.SetActive(false);

        return DOVirtual.DelayedCall(0, () => { });
    }

    /// <summary>
    /// enum 타입의 이름들을 스트링으로 변환해 UI 요소에 연결한다.
    /// </summary>
    /// <typeparam name="T">UI 요소</typeparam>
    /// <param name="type">enum 타입</param>
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);

        if (names.Length == 0) return;

        int startIdx = 0;

        if (!_objectDic.TryGetValue(typeof(T), out UnityEngine.Object[] objects))
        {
            // T key값이 _objects에 없음
            objects = new UnityEngine.Object[names.Length];
            _objectDic[typeof(T)] = objects;
        }
        else
        {
            // T key값이 이미 _objects에 있음: 배열 길이 늘리기
            // UI_Base를 상속받는 클래스로부터의 재상속을 위해 필요
            // 부모 클래스에서 이미 Bind한 타입을 자식 클래스에서 다시 Bind할 수 있게 하기 위함
            // 단, 상속 관계에서 같은 타입을 Bind할 경우 enum의 이름이 겹치면 안되며, Get을 사용할 때 부모 클래스 enum의 Length를 더해주어야 하는 불편함이 있음
            startIdx = objects.Length;
            Array.Resize(ref objects, objects.Length + names.Length);
            _objectDic[typeof(T)] = objects;
        }

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i + startIdx] = GlobalUtility.FindChild(gameObject, names[i], true);
            else
                objects[i + startIdx] = GlobalUtility.FindChild<T>(gameObject, names[i], true);

            if (objects[i + startIdx] == null)
                Debug.LogWarning($"[UI_Base] Failed to bind({names[i]}) in {gameObject.name}");
        }
    }

    // 등록된 enum중에서 해당하는 인덱스의 UI 요소를 가져온다.
    protected T Get<T>(Enum idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects;
        if (_objectDic.TryGetValue(typeof(T), out objects) == false)
        {
            Debug.LogError($"[UI_Base] Get<{typeof(T).Name}>() failed: No UI elements registered!");
            return null;
        }

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
    public static void BindEvent(GameObject go, Action<PointerEventData> action, UIEvent type = UIEvent.Click)
    {
        UI_EventHandler evt = GlobalUtility.GetOrAddComponent<UI_EventHandler>(go);
        GlobalUtility.GetOrAddComponent<GraphicRaycaster>(go);

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
