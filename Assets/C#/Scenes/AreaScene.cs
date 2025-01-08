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
    private void TestInit()
    {
        Quest testQuest = new Quest(Managers.DataMng.QuestDataDict.Values.ToList()[0]);
        if(!Enum.TryParse(testQuest.QuestData.AreaName, out Define.AreaName areaName))
        {
            areaName = Define.AreaName.Forest;
        }

        Managers.ObjectMng.SpawnHeroesOnTest();
        InitArea(areaName, testQuest);
    }

    private void Start()
    {
        // TODO: AreaScene에서 시작하여 플레이 테스트 시에만 실행
        if (Managers.SceneMng.FirstScene != null && Managers.SceneMng.FirstScene is AreaScene)
        {
            TestInit();
        }
    }
    #endregion
}
