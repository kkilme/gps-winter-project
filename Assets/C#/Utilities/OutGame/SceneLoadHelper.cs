using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// 씬 로딩에 도움을 주는 헬퍼 클래스
public static class SceneLoadHelper
{
    // 가짜로 로딩 진행률을 업데이트하는 코루틴
    public static IEnumerator FakeProgress(UI_Loading loadingUI, float to, float duration = 0.1f)
    {
        float elapsed = 0f;
        float from = loadingUI.Progress; // 현재 진행률을 가져옴
        to = Mathf.Max(from, to); // to가 from보다 작으면 from으로 설정

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

    public static IEnumerator LoadSceneWithProgress(string sceneName, UI_Loading loadingUI, float startProgress = 0f, float endProgress = 1f, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        var op = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        op.allowSceneActivation = false; // 로딩이 끝나도 바로 씬 활성화 안됨

        float progress = startProgress;
        float targetProgress = Mathf.Max(startProgress, endProgress - 0.02f); // 실제 endProgress보다 약간 낮은 값으로 설정
        while (op.progress < 0.9f)  // 씬 로딩의 op.progress는 0.9f까지만 올라감
        {
            float normalized = Mathf.Clamp01(op.progress / 0.9f);
            float mapped = Mathf.Lerp(startProgress, targetProgress, normalized);
            progress = Mathf.Lerp(progress, mapped, Time.deltaTime * 4f);
            loadingUI.UpdateProgress(progress);
            yield return null;
        }

        // 씬 로딩이 매우 빠를 경우 위의 루프에서 progress가 targetProgress에 도달하지 못할 수 있음
        // loadingUI.Progress를 직접 사용하여 targetProgress까지 부드럽게 진행률 업데이트
        yield return FakeProgress(loadingUI, targetProgress, 0.4f);

        op.allowSceneActivation = true; // 씬 활성화
        yield return op; // 씬 활성화가 끝날 때까지 대기

        loadingUI.UpdateProgress(endProgress); // 최종적으로 endProgress로 업데이트
    }
}
