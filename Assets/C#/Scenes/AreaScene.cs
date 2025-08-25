using UnityEngine;

public class AreaScene : BaseScene
{
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
        // AreaMapGenerator 초기화 및 맵 생성
        _areaMapGenerator.Init(areaInitContext.AreaName);
        AreaMap map = _areaMapGenerator.GenerateMap();

        // AreaManager 초기화
        AreaManager.Init(map, areaInitContext);
    }

#if UNITY_EDITOR
    #region Test
    private void Start()
    {
        // AreaScene에서 시작하여 플레이 테스트 시에만 실행
        if (Managers.SceneMng.IsFirstScene)
        {
            Managers.SoundMng.PlayBGM("AreaBGM");
            AreaInitContext testContext = new TestAreaInitContext();
            InitArea(testContext);
        }
    }

    public new void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.T))
        {
            CoroutineRunner.Instance.StartCoroutine(Managers.SceneMng.LoadTownScene());
        }
    }
    #endregion
#endif
}
