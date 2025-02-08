using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlacementPhase : UI_Base
{
    private RectTransform _rect;
    enum Buttons
    {
        Button_StartBattle,
    }
    public override void Init()
    {   
        Bind<Button>(typeof(Buttons));
        _rect = GetComponent<RectTransform>();
        GetButton(Buttons.Button_StartBattle).onClick.AddListener(() =>
        {
            Managers.BattleMng.StartBattlePhase();
        });
    }

    public override void Hide()
    {
        _rect.DOAnchorPosY(-500, 0.7f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
            Managers.BattleMng.BattleSceneUI.OnTurnStart();
        });
    }
}
