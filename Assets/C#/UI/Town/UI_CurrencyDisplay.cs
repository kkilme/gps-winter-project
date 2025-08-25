using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CurrencyDisplay : UI_Base
{
    enum Texts
    {
        Text_Gold,
    }

    private RectTransform _goldParentRect;
    private Tween _goldColorTween;
    private Color _goldOriginalColor;

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        _goldParentRect = GetText(Texts.Text_Gold).GetComponentInParent<RectTransform>();
        _goldOriginalColor = GetText(Texts.Text_Gold).color;
        GetText(Texts.Text_Gold).text = Managers.InvMng.Gold.ToString("N0");

        Managers.InvMng.OnGoldChanged -= UpdateGold;
        Managers.InvMng.OnGoldChanged += UpdateGold;
    }

    private void UpdateGold(int goldAmount)
    {
        int currentGold = int.Parse(GetText(Texts.Text_Gold).text.Replace(",", ""));
        _goldColorTween?.Kill();
        GetText(Texts.Text_Gold).color = _goldOriginalColor;
        _goldColorTween = GetText(Texts.Text_Gold).DOColor(currentGold < goldAmount ? Color.green : Color.red, 0.3f).SetLoops(2, LoopType.Yoyo);

        GetText(Texts.Text_Gold).text = goldAmount.ToString("N0");
        LayoutRebuilder.ForceRebuildLayoutImmediate(_goldParentRect); // Preferred Width에 맞추기 위해 Horizontal Layout 강제 업데이트
    }

    private void OnDestroy()
    {
        Managers.InvMng.OnGoldChanged -= UpdateGold;
    }
}
