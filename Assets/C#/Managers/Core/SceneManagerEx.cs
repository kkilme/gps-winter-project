using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public BaseScene FirstScene;
    public BaseScene CurrentScene => GameObject.FindObjectOfType<BaseScene>();

    public void Init() => FirstScene = CurrentScene;

    // type의 이름을 string으로 반환
    private string GetSceneName(SceneType type) => Enum.GetName(typeof(SceneType), type);
  
    // 현재 씬을 T 타입으로 반환
    public T GetCurrentScene<T>() where T : BaseScene => CurrentScene as T;

    public void Clear() => CurrentScene.Clear();
    
    // 전투씬 전환 흐름: Area 카메라 정지 -> 로딩화면 Fade in ->  배틀 씬 로딩 시작 및 완료 -> Area의 빛, 카메라 비활성화 -> 로딩화면 Fade out
    public IEnumerator LoadBattleScene()
    {
        Debug.Log("[SceneManagerEx] Battle Scene Load Start");

        // 로딩화면 생성 및 Fade in
        var loadingUI = Managers.UIMng.MakeGeneralUI<UI_Loading>();
        yield return loadingUI.FadeIn();

        // Battle 씬 로드
        yield return SceneLoadHelper.LoadSceneWithProgress(GlobalValues.BATTLE_SCENE_NAME, loadingUI, 0, LoadSceneMode.Additive);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(GlobalValues.BATTLE_SCENE_NAME));
        Managers.AreaMng.OnBattleSceneLoadFinish();

        // 로딩 화면 fade out, 삭제
        yield return loadingUI.FadeOut();

        Debug.Log("[SceneManagerEx] Battle Scene Load Finish");
    }

    // 전투씬 종료 흐름
    // 1. 전투씬 언로드
    // 2-1. 패배 시: Area 씬 언로드, Town 씬 로드
    // 2-2. 승리 또는 도망 시: Area 씬 활성화
    public IEnumerator EndBattleScene(BattleResultType result)
    {
        Debug.Log($"[SceneManagerEx] Battle Scene End Start, Result: {result}");

        var loadingUI = Managers.UIMng.MakeGeneralUI<UI_Loading>();
        yield return loadingUI.FadeIn();

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
        }
        else
        {
            #if UNITY_EDITOR
            
            if(!SceneManager.GetSceneByName(GlobalValues.AREA_SCENE_NAME).isLoaded)
            {
                // AreaScene이 로드되지 않은 경우: 테스트용으로 BattleScene에서 시작 한 경우임
                Quest quest = new Quest(Managers.DataMng.QuestDataDict.Values.ToList()[0]);
                yield return SceneManager.LoadSceneAsync(GlobalValues.AREA_SCENE_NAME);
                GetCurrentScene<AreaScene>().InitArea(AreaName.Forest, quest);
            }

            #endif

            // AreaScene은 열려있음 → 활성화
            yield return SceneLoadHelper.FakeProgress(loadingUI, 0.3f, 1f);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(GlobalValues.AREA_SCENE_NAME));
            Managers.AreaMng.OnBattleSceneUnloadFinish(result);
        }

        yield return loadingUI.FadeOut();
        Debug.Log("[SceneManagerEx] Battle Scene End Finish");
    }

    public IEnumerator LoadAreaScene(AreaName areaName, Quest quest)
    {
        yield return SceneManager.LoadSceneAsync(GlobalValues.AREA_SCENE_NAME);
        GetCurrentScene<AreaScene>().InitArea(areaName, quest);
    }
}
