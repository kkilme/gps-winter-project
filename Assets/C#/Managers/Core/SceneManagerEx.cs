using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public SceneType FirstScene;
    public SceneType CurrentSceneType;
    private BaseScene _currentScene;
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

    // 전투씬 전환 흐름: LoadBattleScene -> 로딩화면 Fade in 완료 -> OnBattleSceneLoadStart ->  배틀 씬 로딩 시작 및 완료 -> OnBattleSceneLoadFinish -> 로딩화면 Fade out
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

    // 전투씬 종료 흐름
    // 1. 전투씬 언로드
    // 2-1. 영웅 모두 사망: Area 씬 언로드, Town 씬 로드
    // 2-2. 승리 / 도망친 영웅 존재: Area 씬 활성화
    public IEnumerator EndBattleScene(BattleResultType result)
    {
        Debug.Log($"[SceneManagerEx] BattleScene End Start, Result: {result}");

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
                Quest quest = new Quest(Managers.DataMng.QuestDataDict.Values.ToList()[0]);
                yield return SceneManager.LoadSceneAsync(GlobalValues.AREA_SCENE_NAME);
                CurrentSceneType = SceneType.AreaScene;
                GetCurrentScene<AreaScene>().InitArea(AreaName.Forest, quest);
            }
            //////////////////////////////////////////////////////////////////////////////////////////
#endif

            // AreaScene은 열려있음 → 활성화
            yield return SceneLoadHelper.FakeProgress(loadingUI, 0.3f, 1f);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(GlobalValues.AREA_SCENE_NAME));
            CurrentSceneType = SceneType.AreaScene;
            Managers.AreaMng.OnBattleSceneUnloadFinish(result);
        }

        yield return loadingUI.FadeOut();
        Debug.Log("[SceneManagerEx] BattleScene End Finish");
    }

    public IEnumerator LoadAreaScene(AreaName areaName, Quest quest)
    {
        yield return SceneManager.LoadSceneAsync(GlobalValues.AREA_SCENE_NAME);
        CurrentSceneType = SceneType.AreaScene;
        GetCurrentScene<AreaScene>().InitArea(areaName, quest);
    }
}
