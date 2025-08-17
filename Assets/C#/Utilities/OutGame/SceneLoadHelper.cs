using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// 씬 로딩에 도움을 주는 헬퍼 클래스
public static class SceneLoadHelper
{
    // 가짜로 로딩 진행률을 업데이트하는 코루틴
    public static IEnumerator FakeProgress(UI_Loading loadingUI, float from, float to, float duration = 0.1f)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float progress = Mathf.Lerp(from, to, t);
            loadingUI.UpdateProgress(progress);
            yield return null;
        }

        loadingUI.UpdateProgress(to);
    }

    public static IEnumerator LoadSceneWithProgress(string sceneName, UI_Loading loadingUI, float startProgress = 0f, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        var op = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        op.allowSceneActivation = false; // 로딩이 끝나도 바로 씬 활성화 안됨

        float progress = startProgress;
        while (op.progress < 0.9f)  // 씬 로딩의 op.progress는 0.9f까지만 올라감
        {
            progress = Mathf.Lerp(progress, 0.9f, Time.deltaTime * 2f);
            loadingUI.UpdateProgress(progress);
            yield return null;
        }
        loadingUI.UpdateProgress(0.9f);
        yield return FakeProgress(loadingUI, 0.9f, 1f, 0.3f); // progress는 0.9f까지만 올라가므로 강제로 업데이트

        //yield return new WaitForSeconds(1f); // 로딩이 끝나고 잠시 대기

        op.allowSceneActivation = true; // 씬 활성화
        yield return op; // 씬 활성화가 끝날 때까지 대기
    }

}
