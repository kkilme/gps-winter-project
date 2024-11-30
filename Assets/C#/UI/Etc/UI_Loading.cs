using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UI_Loading : UI_Scene
{
    private CanvasGroup _canvasGroup;

    public override void Init()
    {
        base.Init();
		_canvasGroup = GetComponent<CanvasGroup>();
        GetComponent<Canvas>().sortingOrder = 100;
    }

    public IEnumerator Fade(bool isFadeIn)
    {
        if (isFadeIn)
        {
            _canvasGroup.alpha = 1;
            Tween tween = _canvasGroup.DOFade(0f, 1f);
            yield return tween.WaitForCompletion();
            Destroy(gameObject);
        }
        else
        {
            _canvasGroup.alpha = 0;
            Tween tween = _canvasGroup.DOFade(1f, 1f);
            yield return tween.WaitForCompletion();
        }
    }
}
