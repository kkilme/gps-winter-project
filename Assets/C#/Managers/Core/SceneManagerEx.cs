using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public BaseScene FirstScene;
    public BaseScene CurrentScene => GameObject.FindObjectOfType<BaseScene>();

    public void Init()
    {
        FirstScene = CurrentScene;
    }
    // type의 이름을 string으로 반환
    private string GetSceneName(Define.SceneType type)
    {
        return Enum.GetName(typeof(Define.SceneType), type);
    }

    // 현재 씬을 T 타입으로 반환
    public T GetCurrentScene<T>() where T : BaseScene
    {
        return CurrentScene as T;
    }

    // type에 해당하는 Scene을 로드
    public void LoadScene(Define.SceneType type)
    {
        Managers.Clear();

        SceneManager.LoadScene(GetSceneName(type));
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }

    // 전투씬 전환 흐름: 카메라 정지 -> 로딩화면 Fade in 완료 ->  배틀 씬 로딩 시작 및 완료 -> Area의 빛, 카메라 비활성화 -> 로딩화면 Fade out
    public IEnumerator LoadBattleScene()
    {
        Debug.Log("Battle Scene Load Start");
        var sceneName = Define.BATTLE_SCENE_NAME;

        Managers.AreaMng.OnBattleSceneLoadStart(); // 카메라 정지

        UI_Loading loadingScreen = Managers.UIMng.ShowSceneUI<UI_Loading>(); // 로딩화면
        yield return loadingScreen.Fade(false); // fade out

        var battleScene = SceneManager.GetSceneByName(sceneName);
        if (!battleScene.isLoaded)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
        Managers.AreaMng.OnBattleSceneLoadFinish(); // Area의 빛, 카메라 비활성화

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        yield return loadingScreen.Fade(true); // fade in, LoadingUI 삭제
        Debug.Log("Battle Scene Load Finish");
    }

    public IEnumerator UnloadBattleScene()
    {
        var battlesceneName = "TestBattleScene"; // TODO - Test Code
        var areasceneName = "TestAreaScene"; // TODO - Test Code

        UI_Loading loadingScreen = Managers.UIMng.ShowSceneUI<UI_Loading>();
        yield return loadingScreen.Fade(false); // fade out

        var battleScene = SceneManager.GetSceneByName(battlesceneName);
        if (battleScene.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(battlesceneName);
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(areasceneName));
        GetCurrentScene<AreaScene>().OnBattleSceneUnloadFinish.Invoke(); // Battlescene 언로드 된 후 실행


        yield return loadingScreen.Fade(true); // fade in, LoadingUI 삭제
    }

    public IEnumerator LoadAreaScene(Define.AreaName areaName, Quest quest)
    {
        yield return SceneManager.LoadSceneAsync(GetSceneName(Define.SceneType.AreaScene));
        GetCurrentScene<AreaScene>().InitArea(areaName, quest);
    }
}
