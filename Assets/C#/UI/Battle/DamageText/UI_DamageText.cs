using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(UI_Billboard))]
public class UI_DamageText : UI_Base
{
    public TextMeshProUGUI Text { get; private set; }

    public override void Init()
    {
        Text = GetComponentInChildren<TextMeshProUGUI>();
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
