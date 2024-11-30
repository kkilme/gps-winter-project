using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

// 모든 Scene의 조상 클래스
public abstract class BaseScene : MonoBehaviour
{ 
    public Define.SceneType SceneType { get; protected set; } = Define.SceneType.UnknownScene;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        Managers.InputMng.OnUpdate();
    }

    protected virtual void Init()
    {
        // TODO - 나중엔 최초 Scene에서만 실행
        if (!Managers.BattleMng.Initialized)
        {
            Managers.Init();
            Managers.InputMng.Init();
            Managers.DataMng.Init();
            Managers.SoundMng.Init();
            Managers.PoolMng.Init();
            Managers.UIMng.Init();
            Managers.ObjectMng.Init();
            Managers.BattleMng.Init();
            
            Managers.BattleMng.Initialized = true;
        }
        
        Object obj = FindObjectOfType(typeof(EventSystem));
        
        if (obj == null)
            Managers.ResourceMng.Instantiate("UI/EventSystem").name = "@EventSystem";
    }
    
    public abstract void Clear();
}