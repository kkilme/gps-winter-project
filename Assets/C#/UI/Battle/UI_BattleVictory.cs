using DG.Tweening;
using UnityEngine;

public class UI_BattleVictory : UI_Base
{
    enum RectTransforms
    {
        VictoryTitle,
        VictoryDescription
    }

    public override void Init()
    {
        Bind<RectTransform>(typeof(RectTransforms));
        gameObject.SetActive(false);
    }

    public override Tween Show()
    {
        gameObject.SetActive(true);

        RectTransform titleRect = Get<RectTransform>(RectTransforms.VictoryTitle);
        RectTransform descRect = Get<RectTransform>(RectTransforms.VictoryDescription);

        Sequence seq = DOTween.Sequence();

        seq.Append(titleRect.DOScale(new Vector3(.2f, .2f, 1f), .5f).From().SetEase(Ease.InQuad));
        seq.AppendInterval(1f);
        seq.Append(titleRect.DOScale(new Vector3(1f, 1f, 1f), 1f));

        Vector3 temp = titleRect.transform.position;
        titleRect.anchorMax = new Vector2(0, 1);
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.transform.position = temp;

        seq.Join(titleRect.DOAnchorPos(new Vector2(280f, -100f), 1f));
        seq.Append(descRect.DOAnchorPosX(240, 1.5f).SetEase(Ease.OutCirc));

        return seq.Play();
    }
}
    