using System;
using System.Linq;
using UnityEngine;

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
    private AreaMapGenerator _areaMapGenerator;

    public AreaState AreaState
    {
        get => AreaManager.AreaState;
        set => AreaManager.AreaState = value;
    }


    protected override void Init()
    {
        base.Init();
        SceneType = SceneType.AreaScene;

        _areaMapGenerator = GetComponent<AreaMapGenerator>();
    }

    public void InitArea(AreaName areaName, Quest quest)
    {
        AreaName = areaName;
        Quest = quest;

        _areaMapGenerator.Init(areaName);
        AreaMap map = _areaMapGenerator.GenerateMap();

        AreaManager.Init(map);
    }

    public override void Clear()
    {
        Debug.Log("AreaScene Clear!");
    }

    #region Test
    private void TestInit()
    {
        Quest testQuest = new Quest(Managers.DataMng.QuestDataDict.Values.ToList()[0]);
        if (!Enum.TryParse(testQuest.QuestData.AreaName, out AreaName areaName))
        {
            areaName = AreaName.Forest;
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
