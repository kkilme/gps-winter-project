using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(UI_Billboard))]
public class UI_DamageText : UI_Base
{
    public TextMeshProUGUI Text { get; private set; }
    private Camera _camera;

    public override void Init()
    {
        Text = GetComponentInChildren<TextMeshProUGUI>();
        _camera = Camera.main;
    }

    public void Show(int amount, DamageTextDesign design)
    {
        design.ApplyDesign(amount, this);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOMoveY(transform.position.y + 1.5f, 2f).SetEase(Ease.OutQuad));
        sequence.Join(Text.DOFade(0, 4f));
        sequence.OnComplete(() => Managers.ResourceMng.Destroy(gameObject));

        sequence.Play();
    }
}
