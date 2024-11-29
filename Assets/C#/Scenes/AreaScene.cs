using System;
using UnityEngine;
using static Define;

public class AreaScene : BaseScene
{
    private AreaName _areaName;

    public AreaName AreaName
    {
        get => _areaName;
        set
        {
            _areaName = value;
            AreaManager.AreaName = value;
        }
    }
    public Quest Quest { get; private set; }

    private AreaManager AreaManager => Managers.AreaMng;

    public AreaState AreaState
    {
        get => AreaManager.AreaState;
        set => AreaManager.AreaState = value;
    }

    public Action OnBattleSceneLoadStart;
    public Action OnBattleSceneUnloadFinish;
    
    protected override void Init()
    {
        base.Init();
        SceneType = SceneType.AreaScene;
        //InitArea(AreaName.Forest); // TODO: Test code. AreaScene에서 직접 테스트 및 작업 시 필요

        OnBattleSceneLoadStart -= AreaManager.OnBattleSceneLoadStart;
        OnBattleSceneLoadStart += AreaManager.OnBattleSceneLoadStart;
        OnBattleSceneUnloadFinish -= AreaManager.OnBattleSceneUnloadFinish;
        OnBattleSceneUnloadFinish += AreaManager.OnBattleSceneUnloadFinish;
    }

    public void InitArea(AreaName areaName, Quest quest)
    {
        AreaName = areaName;
        Quest = quest;
        AreaManager.InitArea();
    }

    public void LoadBattleScene()
    {   
        AreaManager.FreezeCamera();
        StartCoroutine(Managers.SceneMng.LoadBattleScene());
    }

    public void UnloadBattleScene()
    {
        StartCoroutine(Managers.SceneMng.UnloadBattleScene());
        AreaState = AreaState.Idle;
    }
    
    public override void Clear()
    {
        Debug.Log("AreaScene Clear!");
    }

}
