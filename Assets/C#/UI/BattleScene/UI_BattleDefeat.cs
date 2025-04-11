using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_BattleDefeat : UI_Base
{
    enum RectTransforms
    {
        DefeatTitle,
    }

    enum Buttons
    {
        Button_ReturnToTown,
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
        RectTransform titleRect = Get<RectTransform>(RectTransforms.DefeatTitle);
        Button button = Get<Button>(Buttons.Button_ReturnToTown);
        button.gameObject.SetActive(false);

        Sequence seq = DOTween.Sequence();
        seq.Append(titleRect.DOScale(new Vector3(.2f, .2f, 1f), .7f).From().SetEase(Ease.InQuad));
        seq.AppendInterval(0.5f);
        seq.Append(titleRect.DOAnchorPosY(150f, 1f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            button.gameObject.SetActive(true);
            button.GetComponent<RectTransform>().DOScale(new Vector3(.5f, .5f, .5f), 0.5f).From().SetEase(Ease.OutBack);
            button.onClick.AddListener(() =>
            {
                CoroutineRunner.Instance.StartCoroutine(Managers.SceneMng.EndBattleScene(BattleResultType.Defeat));
            });
        }));

        return seq.Play();
    }
}
    