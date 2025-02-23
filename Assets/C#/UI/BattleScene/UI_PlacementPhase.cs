using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlacementPhase : UI_Base
{
    private RectTransform _rectTransform;
    enum Buttons
    {
        Button_StartBattle,
    }
    public override void Init()
    {   
        Bind<Button>(typeof(Buttons));
        _rectTransform = GetComponent<RectTransform>();
        GetButton(Buttons.Button_StartBattle).onClick.AddListener(() =>
        {
            Managers.BattleMng.StartBattlePhase();
        });
    }

    public override Tween Hide()
    {
        return _rectTransform.DOAnchorPosY(500, 0.7f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
            Managers.BattleMng.BattleSceneUI.OnTurnStart();
        });
    }
}
