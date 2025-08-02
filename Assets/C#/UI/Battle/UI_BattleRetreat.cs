using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 전투에서 후퇴 시의 UI
/// </summary>
public class UI_BattleRetreat : UI_Base
{
    enum RectTransforms
    {
        RetreatTitle,
    }

    enum Buttons
    {
        Button_Return,
    }

    public override void Init()
    {
        Bind<RectTransform>(typeof(RectTransforms));
        Bind<Button>(typeof(Buttons));
        gameObject.SetActive(false);
    }

    public override Tween Show()
    {
        gameObject.SetActive(true);

        RectTransform titleRect = Get<RectTransform>(RectTransforms.RetreatTitle);
        Button button = Get<Button>(Buttons.Button_Return);

        button.gameObject.SetActive(false);

        Sequence seq = DOTween.Sequence();
        seq.Append(titleRect.DOScale(new Vector3(.2f, .2f, 1f), .7f).From().SetEase(Ease.InQuad));
        seq.AppendInterval(0.5f);
        seq.Append(titleRect.DOAnchorPosY(150f, 1f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            button.gameObject.SetActive(true);
            button.GetComponent<RectTransform>().DOScale(new Vector3(.5f, .5f, .5f), 0.5f).From().SetEase(Ease.OutBack);
            // Area로 돌아가는 버튼
            button.onClick.AddListener(() =>
            {
                Managers.BattleMng.UnloadBattleScene(BattleResultType.Retreat);
            });
        }));

        return seq.Play();
    }
}
