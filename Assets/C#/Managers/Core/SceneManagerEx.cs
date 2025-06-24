using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    /// <summary>
    /// 게임을 처음 시작한 씬. 주로 에디터상에서 테스트 용도로 사용됨.
    /// </summary>
    public SceneType FirstScene;
    public SceneType CurrentSceneType;
    private BaseScene _currentScene;
    /// <summary>
    /// 현재 Active 씬의 BaseScene.
    /// </summary>
    public BaseScene CurrentScene
    {
        get
        {
            if(_currentScene == null || _currentScene.SceneType != CurrentSceneType)
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
        CurrentSceneType = FirstScene = CurrentScene.SceneType;
    }

    // 현재 씬을 T 타입으로 반환
    public T GetCurrentScene<T>() where T : BaseScene => CurrentScene as T;

    // 전투씬 전환 흐름 //////////////////
    // 1. 로딩화면 Fade in 완료
    // 2. AreaManager.OnBattleSceneLoadStart()
    // 3. 배틀 씬 로딩 시작 및 완료
    // 4. AreaManager.OnBattleSceneLoadFinish()
    // 5. 로딩화면 Fade out
    /// /////////////////////////////////
    public IEnumerator LoadBattleScene(AreaManager areaManager)
    {
        Debug.Log("[SceneManagerEx] BattleScene Load Start");

        // 로딩화면 생성 및 Fade in
        var loadingUI = Managers.UIMng.MakeGeneralUI<UI_Loading>();
        yield return loadingUI.FadeIn();

        areaManager.OnBattleSceneLoadStart();

        // Battle 씬 로드 및 활성화
        yield return SceneLoadHelper.LoadSceneWithProgress(GlobalValues.BATTLE_SCENE_NAME, loadingUI, 0, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(GlobalValues.BATTLE_SCENE_NAME));
        CurrentSceneType = SceneType.BattleScene;

        areaManager.OnBattleSceneLoadFinish();

        // 로딩 화면 fade out, 삭제
        yield return loadingUI.FadeOut();

        Debug.Log("[SceneManagerEx] BattleScene Load Finish");
    }

    // 전투씬 종료 흐름 //////////////////
    // 1. 로딩화면 Fade in 완료
    // 2. 전투씬 언로드
    // 3-1. 영웅 모두 사망시: Area 씬 언로드, Town 씬 로드
    // 4-2. 승리 / 도망친 영웅 존재시: Area 씬 활성화
    // 5. 로딩화면 Fade out
    /////////////////////////////////////
    public IEnumerator EndBattleScene(BattleResultType result)
    {
        Debug.Log($"[SceneManagerEx] BattleScene End Start, Result: {result}");

        // 로딩화면 생성 및 Fade in
        var loadingUI = Managers.UIMng.MakeGeneralUI<UI_Loading>();
        yield return loadingUI.FadeIn();
        Managers.BattleMng.Clear();

        // BattleScene 언로드
        yield return SceneManager.UnloadSceneAsync(GlobalValues.BATTLE_SCENE_NAME);
        yield return SceneLoadHelper.FakeProgress(loadingUI, 0, 0.3f);

        if (result == BattleResultType.Defeat)
        {
            // AreaScene 언로드
            if (SceneManager.GetSceneByName(GlobalValues.AREA_SCENE_NAME).isLoaded)
                yield return SceneManager.UnloadSceneAsync(GlobalValues.AREA_SCENE_NAME);
            yield return SceneLoadHelper.FakeProgress(loadingUI, 0.3f, 0.6f);

            // TownScene 로드
            yield return SceneLoadHelper.LoadSceneWithProgress(GlobalValues.TOWN_SCENE_NAME, loadingUI, 0.6f);

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(GlobalValues.TOWN_SCENE_NAME));
            CurrentSceneType = SceneType.TownScene;
        }
        else
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

            // AreaScene은 열려있음 → 활성화
            yield return SceneLoadHelper.FakeProgress(loadingUI, 0.3f, 1f);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(GlobalValues.AREA_SCENE_NAME));
            CurrentSceneType = SceneType.AreaScene;
            Managers.AreaMng.OnBattleSceneUnloadFinish(result);
        }

        // 로딩 화면 fade out
        yield return loadingUI.FadeOut();
        Debug.Log("[SceneManagerEx] BattleScene End Finish");
    }

    // TownScene -> AreaScene 전환 흐름 //////////////////
    // 1. 로딩화면 Fade in 완료
    // 2. Area 씬 로드 및 활성화
    // 3. Area 씬 초기화 (AreaInitContext 전달)
    // 4. 로딩화면 Fade out
    ///////////////////////////////////////////////////
    public IEnumerator LoadAreaScene(AreaInitContext areaInitContext)
    {
        Debug.Log($"[SceneManagerEx] AreaScene Load Start, Quest: {areaInitContext.Quest.QuestData.Name}");

        // 로딩화면 생성 및 Fade in
        var loadingUI = Managers.UIMng.MakeGeneralUI<UI_Loading>();
        yield return loadingUI.FadeIn();

        // Area 씬 로드 및 활성화
        yield return SceneLoadHelper.LoadSceneWithProgress(GlobalValues.AREA_SCENE_NAME, loadingUI);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(GlobalValues.AREA_SCENE_NAME));
        CurrentSceneType = SceneType.AreaScene;

        // Area 씬 초기화
        GetCurrentScene<AreaScene>().InitArea(areaInitContext);

        // 로딩 화면 fade out
        yield return loadingUI.FadeOut();
    }
}
