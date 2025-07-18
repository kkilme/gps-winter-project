using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 게임 내 모든 입력 처리
public class InputManager
{
    public Action KeyAction;
    public Action<MouseEvent> MouseAction;
    public Action PointerOverGameObjectAction;

    private bool _pressed;
    private float _pressedTime;

    public void Init()
    {
        KeyAction = null;
        MouseAction = null;
        
        _pressed = false;
        _pressedTime = 0;
    }

    public void AddKeyAction(Action action)
    {
        KeyAction -= action;
        KeyAction += action;
    }

    public void AddMouseAction(Action<MouseEvent> action)
    {
        MouseAction -= action;
        MouseAction += action;
    }

    public void AddPointerOverGameObjectAction(Action action)
    {
        PointerOverGameObjectAction -= action;
        PointerOverGameObjectAction += action;
    }

    public void RemoveKeyAction(Action action)
    {
        KeyAction -= action;
    }

    public void RemoveMouseAction(Action<MouseEvent> action)
    {
        MouseAction -= action;
    }

    public void RemovePointerOverGameObjectAction(Action action)
    {
        PointerOverGameObjectAction -= action;
    }

    // 입력이 없다면 바로 리턴, 입력이 있다면 KeyAction/MouseAction을 Invoke
    public void OnUpdate()
    {
        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject())
        {
            PointerOverGameObjectAction?.Invoke(); 
            return;
        }

        if (KeyAction != null && Input.anyKey)
        {
            KeyAction.Invoke();
        }

        if (MouseAction != null)
        {   
            MouseAction?.Invoke(MouseEvent.Hover);
            if (Input.GetMouseButton(0))
            {
                if (!_pressed)
                {
                    MouseAction?.Invoke(MouseEvent.PointerDown);
                    _pressedTime = Time.time;
                }

                MouseAction?.Invoke(MouseEvent.Press);
                _pressed = true;
            }
            else
            {
                if (_pressed)
                {
                    if (Time.time < _pressedTime + 0.2f)
                        MouseAction?.Invoke(MouseEvent.Click);
                    MouseAction?.Invoke(MouseEvent.PointerUp);

                }
                
                _pressed = false;
                _pressedTime = 0;
            }
        }
    }
    
    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
    }
}

/* 사용예제
    void Start()
    {
        Managers.InputMng.KeyAction += OnKeyboard;
    }

    void OnKeyboard()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.forward * Time.deltaTime * mSpeed;
        }
    }
*/
