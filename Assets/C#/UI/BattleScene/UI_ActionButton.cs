using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ActionButton : MonoBehaviour
{
    private UI_BattleActionPanel _actionPanel;
    private BaseAction _action;

    public void Init(BaseAction action)
    {
        var Button = gameObject.GetOrAddComponent<Button>();

        _actionPanel = Managers.BattleMng.BattleSceneUI.BattleActionPanel;
        _action = action;

        Button.onClick.AddListener(OnClickActionButton);
        gameObject.BindEvent(OnEnterActionButton, Define.UIEvent.Enter);
    }

    protected void OnClickActionButton()
    {
        Managers.BattleMng.CurrentTurnCreature.CurrentAction = _action;
    }

    protected void OnEnterActionButton(PointerEventData data)
    {
        _actionPanel.OnMouseActionIconEntered?.Invoke(_action);
    }
}