using System;
using System.Linq;
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

    public Action OnBattleSceneUnloadFinish;

    protected override void Init()
    {
        base.Init();
        SceneType = Define.SceneType.AreaScene;
        //Init(AreaName.Forest); // TODO: Test code. AreaScene에서 직접 테스트 및 작업 시 필요

        OnBattleSceneUnloadFinish -= AreaManager.OnBattleSceneUnloadFinish;
        OnBattleSceneUnloadFinish += AreaManager.OnBattleSceneUnloadFinish;

        _areaMapGenerator = GetComponent<AreaMapGenerator>();
    }

    public void InitArea(Define.AreaName areaName, Quest quest)
    {
        AreaName = areaName;
        Quest = quest;

        _areaMapGenerator.Init(areaName);
        AreaMap map = _areaMapGenerator.GenerateMap();

        AreaManager.Init(map);
    }

    public void LoadBattleScene()
    {
        StartCoroutine(Managers.SceneMng.LoadBattleScene());
    }

    public void UnloadBattleScene()
    {
        StartCoroutine(Managers.SceneMng.UnloadBattleScene());
    }

    public override void Clear()
    {
        Debug.Log("AreaScene Clear!");
    }

    #region Test
    // AreaScene에서 직접 실행 시 테스트용 코드
    private void TestInit()
    {
        Quest testQuest = new Quest(Managers.DataMng.QuestDataDict.Values.ToList()[0]);
        if(!Enum.TryParse(testQuest.QuestData.AreaName, out Define.AreaName areaName))
        {
            areaName = Define.AreaName.Forest;
        }

        AreaName = areaName;
        Quest = testQuest;

        _areaMapGenerator.Init(areaName);
        AreaMap map = _areaMapGenerator.GenerateMap();
        AreaManager.Init(map, true);
    }

    private void Start()
    {
        if (Managers.SceneMng.CurrentScene is AreaScene)
        {
            TestInit();
        }
    }
    #endregion
}
