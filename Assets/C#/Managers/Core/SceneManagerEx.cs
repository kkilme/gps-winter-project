using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    /// <summary>
    /// 현재 씬 이전에 활성화되어있던 씬. 첫 시작 시 UnknownScene이며, 이를 통해 테스트 환경 구현
    /// </summary>
    public SceneType LastSceneType;
    public SceneType CurrentSceneType;
    private BaseScene _currentScene;
    /// <summary>
    /// 현재 Active 씬의 BaseScene.
    /// </summary>
    public BaseScene CurrentScene
    {
        get
        {
            if (_currentScene == null || _currentScene.SceneType != CurrentSceneType)
            {
                var activeScene = SceneManager.GetActiveScene();
                var rootObjects = activeScene.GetRootGameObjects();

                foreach (var rootObject in rootObjects)
                {
                    if (rootObject.TryGetComponent<BaseScene>(out var baseScene))
                    {
                        _currentScene = baseScene;
                        CurrentSceneType = _currentScene.SceneType;
                        return _currentScene;
                    }
                }
                Debug.LogError($"[SceneManagerEx] BaseScene not found in scene: {activeScene.name}");
                return null;
            }
            return _currentScene;
        }
    }

    public void Init()
    {
        LastSceneType = SceneType.UnknownScene;
        CurrentSceneType = CurrentScene.SceneType;
    }

    // 현재 씬을 T 타입으로 반환
    public T GetCurrentScene<T>() where T : BaseScene => CurrentScene as T;

    // 전투씬 전환 흐름 //////////////////
    // 1. 로딩화면 Fade in 완료
    // 2. AreaManager.OnBattleSceneLoadStart()
    // 3. 배틀 씬 로딩 시작 및 완료
    // 4. AreaManager.OnBattleSceneLoadFinish()
    // 5. monsterSquadId를 받아와 BattleManager.Init()
    // 5. 로딩화면 Fade out
    /// /////////////////////////////////
    public IEnumerator LoadBattleScene(AreaManager areaManager, BattleType battleType)
    {
        Debug.Log("[SceneManagerEx] BattleScene Load Start");

        LastSceneType = SceneType.AreaScene;

        // BGM 페이드 아웃
        Managers.SoundMng.FadeoutBGM();

        // 로딩화면 생성 및 Fade in
        var loadingUI = Managers.UIMng.MakeGeneralUI<UI_Loading>();
        yield return loadingUI.FadeIn();

        areaManager.OnBattleSceneLoadStart();

        // Battle 씬 로드 및 활성화
        yield return SceneLoadHelper.LoadSceneWithProgress(GlobalValues.BATTLE_SCENE_NAME, loadingUI, 0, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(GlobalValues.BATTLE_SCENE_NAME));
        CurrentSceneType = SceneType.BattleScene;

        areaManager.OnBattleSceneLoadFinish();

        // battleSquadId 가져오기
        var squadId = battleType switch
        {
            BattleType.Normal => areaManager.GetRandomMonsterSquadId(),
            BattleType.Boss => areaManager.AreaData.BossSquadId,
            _ => throw new NotImplementedException()
        };

        // BattleManager 초기화
        Managers.BattleMng.Init(squadId, areaManager.AreaData.BattleFieldName);

        Managers.SoundMng.PlayBGM("BattleBGM");

        // 로딩 화면 fade out, 삭제
        yield return loadingUI.FadeOut();

        Debug.Log("[SceneManagerEx] BattleScene Load Finish");
    }

    // 전투씬 종료 흐름 //////////////////
    // 1. 로딩화면 Fade in 완료
    // 2. 전투씬 언로드
    // 3-1. 영웅 모두 사망시(전투 패배 시): Area 씬 언로드, Town 씬 로드
    // 4-2. 승리 / 도망친 영웅 존재시: Area 씬 활성화
    // 5. 로딩화면 Fade out
    /////////////////////////////////////
    public IEnumerator EndBattleScene(BattleResultType result)
    {
        Debug.Log($"[SceneManagerEx] BattleScene End Start, Result: {result}");

        LastSceneType = SceneType.BattleScene;

        // BGM 페이드 아웃
        Managers.SoundMng.FadeoutBGM();

        // 로딩화면 생성 및 Fade in
        var loadingUI = Managers.UIMng.MakeGeneralUI<UI_Loading>();
        yield return loadingUI.FadeIn();

        // BattleScene 언로드
        yield return SceneManager.UnloadSceneAsync(GlobalValues.BATTLE_SCENE_NAME);
        yield return SceneLoadHelper.FakeProgress(loadingUI, 0, 0.1f);

        if (result == BattleResultType.Defeat) // 전투에서 패배 시 - TownScene으로
        {
            // AreaScene 언로드
            if (SceneManager.GetSceneByName(GlobalValues.AREA_SCENE_NAME).isLoaded)
            {
                Managers.AreaMng.Clear();
                yield return SceneManager.UnloadSceneAsync(GlobalValues.AREA_SCENE_NAME);
            }
            yield return SceneLoadHelper.FakeProgress(loadingUI, 0.1f, 0.2f);

            // TownScene 로드
            yield return SceneLoadHelper.LoadSceneWithProgress(GlobalValues.TOWN_SCENE_NAME, loadingUI, 0.2f);

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(GlobalValues.TOWN_SCENE_NAME));
            CurrentSceneType = SceneType.TownScene;
            Managers.TownMng.Init();

            Managers.SoundMng.PlayBGM("TownBGM");

            // 로딩 화면 fade out
            yield return loadingUI.FadeOut();
            Debug.Log("[SceneManagerEx] BattleScene End Finish");
        }
        else // 전투 도망 또는 승리 시 - Area 복귀
        {
#if UNITY_EDITOR
            //////////////////////////// For Test //////////////////////////////////////////////////
            if (!SceneManager.GetSceneByName(GlobalValues.AREA_SCENE_NAME).isLoaded)
            {
                // AreaScene이 로드되지 않은 경우: 테스트용으로 BattleScene에서 시작 한 경우임
                // 임의로 Area 생성
                yield return SceneManager.LoadSceneAsync(GlobalValues.AREA_SCENE_NAME);
                CurrentSceneType = SceneType.AreaScene;
                AreaInitContext context = new TestAreaInitContext();
                GetCurrentScene<AreaScene>().InitArea(context);
            }
            //////////////////////////////////////////////////////////////////////////////////////////
#endif

            Managers.SoundMng.PlayBGM("AreaBGM");

            // AreaScene은 열려있음 → 활성화
            yield return SceneLoadHelper.FakeProgress(loadingUI, 0.1f, 1f);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(GlobalValues.AREA_SCENE_NAME));
            CurrentSceneType = SceneType.AreaScene;

            CoroutineRunner.Instance.StartCoroutine(Managers.AreaMng.OnBattleSceneUnloadFinish(loadingUI));
            Debug.Log("[SceneManagerEx] BattleScene End Finish");
        }

    }

    // TownScene -> AreaScene 전환 //////////////////
    // 1. 로딩화면 Fade in 완료
    // 2. Area 씬 로드 및 활성화
    // 3. Area 씬 초기화 (AreaInitContext 전달)
    // 4. 로딩화면 Fade out
    ///////////////////////////////////////////////////
    public IEnumerator LoadAreaScene(AreaInitContext areaInitContext)
    {
        Debug.Log($"[SceneManagerEx] AreaScene Load Start, Quest: {areaInitContext.Quest.QuestData.Name}");
        LastSceneType = SceneType.TownScene;

        // BGM 페이드 아웃
        Managers.SoundMng.FadeoutBGM();

        // 로딩화면 생성 및 Fade in
        var loadingUI = Managers.UIMng.MakeGeneralUI<UI_Loading>();
        yield return loadingUI.FadeIn();

        // Area 씬 로드 및 활성화
        yield return SceneLoadHelper.LoadSceneWithProgress(GlobalValues.AREA_SCENE_NAME, loadingUI);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(GlobalValues.AREA_SCENE_NAME));
        CurrentSceneType = SceneType.AreaScene;

        // Area 씬 초기화
        GetCurrentScene<AreaScene>().InitArea(areaInitContext);

        // BGM 재생
        Managers.SoundMng.PlayBGM("AreaBGM");

        // 로딩 화면 fade out
        yield return loadingUI.FadeOut();
    }

    // AreaScene -> TownScene 전환 //////////////////
    // 1. 로딩화면 Fade in 완료
    // 2. Town 씬 로드 및 활성화
    // 3. Area 씬 언로드
    // 4. Town 씬 초기화
    // 5. 로딩화면 Fade out
    /////////////////////////////////////////////////
    public IEnumerator LoadTownScene()
    {
        Debug.Log($"[SceneManagerEx] TownScene Load Start");
        LastSceneType = SceneType.AreaScene;

        // BGM 페이드 아웃
        Managers.SoundMng.FadeoutBGM();

        // 로딩화면 생성 및 Fade in
        var loadingUI = Managers.UIMng.MakeGeneralUI<UI_Loading>();
        yield return loadingUI.FadeIn();

        // TownScene 로드
        yield return SceneLoadHelper.LoadSceneWithProgress(GlobalValues.TOWN_SCENE_NAME, loadingUI, 0f, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(GlobalValues.TOWN_SCENE_NAME));

        // AreaScene 언로드
        if (SceneManager.GetSceneByName(GlobalValues.AREA_SCENE_NAME).isLoaded)
        {
            Managers.AreaMng.Clear();
            yield return SceneManager.UnloadSceneAsync(GlobalValues.AREA_SCENE_NAME);
        }

        CurrentSceneType = SceneType.TownScene;
        Managers.TownMng.Init();

        Managers.SoundMng.PlayBGM("TownBGM");

        // 로딩 화면 fade out
        yield return loadingUI.FadeOut();
    }
}
