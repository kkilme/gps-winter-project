using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ActionButton : UI_Base
{
    private UI_BattleActionPanel _actionPanel;
    private BattleSkill _skill;

    public override void Init()
    {
        var Button = gameObject.GetOrAddComponent<Button>();

        _actionPanel = Managers.BattleMng.UI.ActionPanel;

        Button.onClick.AddListener(OnClick);
        gameObject.BindEvent(OnMouseEnterEvent, UIEvent.Enter);
    }

    public void SetSkill(BattleSkill skill)
    {
        _skill = skill;
    }

    protected void OnClick()
    {
        Managers.BattleMng.SetAction(_skill);
    }

    protected void OnMouseEnterEvent(PointerEventData data)
    {   
        _actionPanel.ShowSkillInfo(_skill);
    }
}