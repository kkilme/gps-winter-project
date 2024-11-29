using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.PlayerSettings;

// UI가 입력(클릭, 드래그)을 받을 수 있게 해주는 Class
public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Action<PointerEventData> OnClickHandler = null;
    public Action<PointerEventData> OnDragHandler = null;
    public Action<PointerEventData> OnEnterHandler = null;
    public Action<PointerEventData> OnExitHandler = null;
    public Action<PointerEventData> OnStayHandler = null;
    public Action<PointerEventData> OnDoubleClickHandler = null;

    private bool _isEnter;

    // OnClickHandler에 등록된 함수 모두 실행
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickHandler != null)
            OnClickHandler.Invoke(eventData);

        // 더블 클릭인지 체크
        if (eventData.clickCount == 2)
            OnDoubleClickHandler?.Invoke(eventData);
    }

    // OnDragHandler에 등록된 함수 모두 실행
    public void OnDrag(PointerEventData eventData)
    {
        if (OnDragHandler != null)
            OnDragHandler.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnEnterHandler != null)
            OnEnterHandler.Invoke(eventData);

        _isEnter = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (OnExitHandler != null)
            OnExitHandler.Invoke(eventData);

        _isEnter = false;
    }

    private void Update()
    {
        if (_isEnter)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            OnStayHandler?.Invoke(pointerEventData);
        }
    }
}