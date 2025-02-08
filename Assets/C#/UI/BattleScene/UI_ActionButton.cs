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

        Button.onClick.AddListener(OnClick);
        gameObject.BindEvent(OnMouseEnterEvent, GlobalEnums.UIEvent.Enter);
    }

    protected void OnClick()
    {
        Managers.BattleMng.SetAction(_action);
    }

    protected void OnMouseEnterEvent(PointerEventData data)
    {   
        _actionPanel.ShowActionInfo(_action);
    }
}