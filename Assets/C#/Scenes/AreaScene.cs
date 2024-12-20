using System;
using UnityEngine;

public class AreaScene : BaseScene
{
    private Define.AreaName _areaName;

    public Define.AreaName AreaName
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
    private AreaMapGenerator _areaMapGenerator;
    public Define.AreaState AreaState
    {
        get => AreaManager.AreaState;
        set => AreaManager.AreaState = value;
    }

    public Action OnBattleSceneLoadStart;
    public Action OnBattleSceneUnloadFinish;
    
    protected override void Init()
    {
        base.Init();
        SceneType = Define.SceneType.AreaScene;
        //InitArea(AreaName.Forest); // TODO: Test code. AreaScene에서 직접 테스트 및 작업 시 필요

        OnBattleSceneLoadStart -= AreaManager.OnBattleSceneLoadStart;
        OnBattleSceneLoadStart += AreaManager.OnBattleSceneLoadStart;
        OnBattleSceneUnloadFinish -= AreaManager.OnBattleSceneUnloadFinish;
        OnBattleSceneUnloadFinish += AreaManager.OnBattleSceneUnloadFinish;

        _areaMapGenerator = GetComponent<AreaMapGenerator>();
    }

    public void InitArea(Define.AreaName areaName, Quest quest)
    {
        AreaName = areaName;
        Quest = quest;
        AreaManager.InitArea();

        _areaMapGenerator.Init(areaName);
        _areaMapGenerator.GenerateMap();
    }

    public void LoadBattleScene()
    {   
        AreaManager.FreezeCamera();
        StartCoroutine(Managers.SceneMng.LoadBattleScene());
    }

    public void UnloadBattleScene()
    {
        StartCoroutine(Managers.SceneMng.UnloadBattleScene());
        AreaState = Define.AreaState.Idle;
    }
    
    public override void Clear()
    {
        Debug.Log("AreaScene Clear!");
    }

}
