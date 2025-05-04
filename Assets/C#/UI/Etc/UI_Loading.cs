using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class UI_Loading : UI_Base
{
    private CanvasGroup _canvasGroup;

    enum Sliders
    {
        Slider_Progress
    }

    enum Texts
    {
        Text_Loading
    }

    private Slider _progressBar;
    private TextMeshProUGUI _progressText;

    public override void Init()
    {
		_canvasGroup = GetComponent<CanvasGroup>();
        Bind<Slider>(typeof(Sliders));
        Bind<TextMeshProUGUI>(typeof(Texts));
        _progressBar = Get<Slider>(Sliders.Slider_Progress);
        _progressText = Get<TextMeshProUGUI>(Texts.Text_Loading);
        _progressBar.value = 0;
        _progressText.text = "Loading... 0%";
        _progressBar.gameObject.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator FadeIn()
    {
        _canvasGroup.alpha = 0;
        yield return _canvasGroup.DOFade(1f, 1.5f).WaitForCompletion();
        _progressBar.gameObject.SetActive(true);
    }

    public IEnumerator FadeOut()
    {
        _canvasGroup.alpha = 1f;
        yield return _canvasGroup.DOFade(0f, 1.5f).WaitForCompletion();
        Destroy(gameObject); // Fade out 후 오브젝트 삭제
    }

    public void UpdateProgress(float progress)
    {
        progress = Mathf.Clamp01(progress);
        _progressBar.value = progress;
        if (progress >= 0.99f) progress = 1f;
        _progressText.text = $"Loading... {(int)(progress * 100)}%";
    }
}
