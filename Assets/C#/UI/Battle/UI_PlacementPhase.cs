using DG.Tweening;
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
            GetButton(Buttons.Button_StartBattle).interactable = false;
            Managers.InputMng.RemovePointerOverGameObjectAction(Managers.BattleMng.BattleInputHandler.OnDragEnd_PlacementPhase);
            Managers.InputMng.RemoveMouseAction(Managers.BattleMng.BattleInputHandler.HandleMouseOnPlacementPhase);
            Hide();
        });
    }

    public override Tween Hide()
    {
        return _rectTransform.DOAnchorPosY(500, 0.7f).SetEase(Ease.InBack).OnComplete(() =>
        {
            Managers.BattleMng.StartBattlePhase();
            gameObject.SetActive(false);
        });
    }
}
