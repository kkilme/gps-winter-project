using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public BaseScene CurrentScene => GameObject.FindObjectOfType<BaseScene>();

    // type의 이름을 string으로 반환
    private string GetSceneName(Define.Scene type)
    {
        return System.Enum.GetName(typeof(Define.Scene), type);
    }

    // 현재 씬을 T 타입으로 반환
    public T GetCurrentScene<T>() where T : BaseScene
    {   
        return CurrentScene as T;
    }

    // type에 해당하는 Scene을 로드
    public void LoadScene(Define.Scene type)
    {
        Managers.Clear();
        
        SceneManager.LoadScene(GetSceneName(type));
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }

    
    public IEnumerator LoadBattleScene()
    {
        var sceneName = "TestBattleScene"; // TODO - Test Code

        UI_Loading loadingScreen = Managers.UIMng.ShowSceneUI<UI_Loading>();

        yield return loadingScreen.Fade(false); // fade out

        GetCurrentScene<AreaScene>().OnBattleSceneLoadStart.Invoke(); // 화면이 완전히 fade out 된 후 실행

        var battleScene = SceneManager.GetSceneByName(sceneName);
        if (!battleScene.isLoaded)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        SceneManager.MoveGameObjectToScene(GameObject.Find("@Players"), SceneManager.GetSceneByName(sceneName)); // 전투씬으로 플레이어 옮김

        yield return loadingScreen.Fade(true); // fade in, LoadingUI 삭제
    }

    public IEnumerator UnloadBattleScene()
    {
        var battlesceneName = "TestBattleScene"; // TODO - Test Code
        var areasceneName = "TestAreaScene"; // TODO - Test Code

        UI_Loading loadingScreen = Managers.UIMng.ShowSceneUI<UI_Loading>();
        yield return loadingScreen.Fade(false); // fade out

        SceneManager.MoveGameObjectToScene(GameObject.Find("@Players"), SceneManager.GetSceneByName(areasceneName)); // 구역씬으로 플레이어 복귀

        var battleScene = SceneManager.GetSceneByName(battlesceneName);
        if (battleScene.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(battlesceneName);
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(areasceneName));
        GetCurrentScene<AreaScene>().OnBattleSceneUnloadFinish.Invoke(); // Battlescene 언로드 된 후 실행
    

        yield return loadingScreen.Fade(true); // fade in, LoadingUI 삭제
    }
}
