using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CreatureTurnFrame : UI_Base
{
    private Tweener _moveTweener;
    private Tweener _sizeTweener;
    private Tweener _blinkTweener;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Image _bg;
    private Color _original;

    enum Images
    {
        bg,
        Creature_Image
    }

    override public void Init()
    {
        Bind<Image>(typeof(Images));
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _bg = Get<Image>(Images.bg);
        _original = _bg.color;
    }

    public void Setup(Creature creature)
    {
        _rectTransform.anchoredPosition = new Vector2(0, 0);
        Get<Image>(Images.Creature_Image).sprite = Managers.ResourceMng.Load<Sprite>($"Textures/ModelSprites/{creature.CreatureData.Name}_Front");
    }

    public void MoveTo(float x)
    {
        if (_moveTweener != null)
            _moveTweener.Kill();
        _moveTweener = _rectTransform.DOAnchorPosX(x, 0.5f);
    }

    public void Resize(float size)
    {
        if (_sizeTweener != null)
            _sizeTweener.Kill();
        _sizeTweener = _rectTransform.DOSizeDelta(new Vector2(size, size), 0.5f);
    }

    public override Tween Hide()
    {
        return _canvasGroup.DOFade(0, 0.5f).OnComplete(() => gameObject.SetActive(false));
    }

    public void StartBlinking()
    {
        _blinkTweener = _bg.DOColor(Color.yellow, 1f).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopBlinking()
    {
        if (_blinkTweener != null)
            _blinkTweener.Kill();
        _bg.color = _original;
    }
}
