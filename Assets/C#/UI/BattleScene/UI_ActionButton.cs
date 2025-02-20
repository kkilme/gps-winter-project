using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ActionButton : MonoBehaviour
{
    private UI_BattleActionPanel _actionPanel;
    private BaseSkill _skill;

    public void Init(BaseSkill skill)
    {
        var Button = gameObject.GetOrAddComponent<Button>();

        _actionPanel = Managers.BattleMng.BattleSceneUI.BattleActionPanel;
        _skill = skill;

        Button.onClick.AddListener(OnClick);
        gameObject.BindEvent(OnMouseEnterEvent, UIEvent.Enter);
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