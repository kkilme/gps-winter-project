using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Area/전투에서 패배했을 시의 UI
/// </summary>
public class UI_Defeat : UI_Popup
{
    enum RectTransforms
    {
        DefeatTitle,
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
        Show();
    }

    public override Tween Show()
    {
        gameObject.SetActive(true);

        RectTransform titleRect = Get<RectTransform>(RectTransforms.DefeatTitle);
        Button button = Get<Button>(Buttons.Button_Return);

        button.gameObject.SetActive(false);

        Sequence seq = DOTween.Sequence();
        seq.Append(titleRect.DOScale(new Vector3(.2f, .2f, 1f), .7f).From().SetEase(Ease.InQuad));
        seq.AppendInterval(0.5f);
        seq.Append(titleRect.DOAnchorPosY(150f, 1f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            button.gameObject.SetActive(true);
            button.GetComponent<RectTransform>().DOScale(new Vector3(.5f, .5f, .5f), 0.5f).From().SetEase(Ease.OutBack);
            // 마을로 돌아가는 버튼
            button.onClick.AddListener(() =>
            {
                var currentSceneType = Managers.SceneMng.CurrentSceneType;
                if (currentSceneType == SceneType.BattleScene)
                    Managers.BattleMng.UnloadBattleScene(BattleResultType.Defeat);
                else if (currentSceneType == SceneType.AreaScene)
                    Managers.AreaMng.LoadTownScene();
            });
        }));

        return seq.Play();
    }
}
