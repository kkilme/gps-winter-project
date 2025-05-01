using System;
using System.Linq;
using UnityEngine;

public class AreaScene : BaseScene
{
    public Quest Quest { get; private set; }

    private AreaManager AreaManager => Managers.AreaMng;
    private AreaMapGenerator _areaMapGenerator;

    protected override void Init()
    {
        base.Init();
        SceneType = SceneType.AreaScene;

        _areaMapGenerator = GetComponent<AreaMapGenerator>();
    }

    public void InitArea(AreaName areaName, Quest quest)
    {
        Quest = quest;

        _areaMapGenerator.Init(areaName);
        AreaMap map = _areaMapGenerator.GenerateMap();

        AreaManager.Init(areaName, map);
    }

    #region Test
    private void TestInit()
    {
        Quest testQuest = new Quest(Managers.DataMng.QuestDataDict.Values.ToList()[0]);
        if (!Enum.TryParse(testQuest.QuestData.AreaName, out AreaName areaName))
        {
            areaName = AreaName.Forest;
        }

        Managers.HeroMng.AddHeroesOnTest();
        InitArea(areaName, testQuest);
    }

    private void Start()
    {
        // TODO: AreaScene에서 시작하여 플레이 테스트 시에만 실행
        if (Managers.SceneMng.FirstScene is AreaScene)
        {
            TestInit();
        }
    }

    // TOOD: for test
    public new void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.D))
        {
            Managers.HeroMng.HeroParty.Heroes[0].TakeDamage(1, DamageTextType.NormalDamage);
        }
    }
    #endregion
}
