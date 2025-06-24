using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AreaScene : BaseScene
{
    public Quest Quest { get; private set; }

    private AreaManager AreaManager => Managers.AreaMng;
    private AreaMapGenerator _areaMapGenerator;

    protected override void Init()
    {
        SceneType = SceneType.AreaScene;
        base.Init();

        _areaMapGenerator = GetComponent<AreaMapGenerator>();
    }

    public void InitArea(AreaInitContext areaInitContext)
    {
        Quest = areaInitContext.Quest;
        
        // AreaMapGenerator 초기화 및 맵 생성
        _areaMapGenerator.Init(areaInitContext.AreaName);
        AreaMap map = _areaMapGenerator.GenerateMap();

        // AreaManager 초기화
        AreaManager.Init(map, areaInitContext);
    }

    #region Test
    private void Start()
    {
        // TODO: AreaScene에서 시작하여 플레이 테스트 시에만 실행
        if (Managers.SceneMng.FirstScene == SceneType.AreaScene)
        {
            AreaInitContext testContext = new TestAreaInitContext();
            InitArea(testContext);
        }
    }

    // TOOD: for test
    public new void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.D))
        {
            
        }
    }
    #endregion
}
