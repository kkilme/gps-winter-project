using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ActionButton : UI_Base
{
    public Button Button { get; protected set; }
    
    public UI_BattleActionPanel BattleActionPanel { get; set; }
    
    public BaseAction Action { get; set; }
    
    public override void Init()
    {
        Button = gameObject.GetOrAddComponent<Button>();
        
        Button.onClick.AddListener(OnClickActionButton);
        gameObject.BindEvent(OnEnterActionButton, Define.UIEvent.Enter);
    }

    protected void OnClickActionButton()
    {
        BattleActionPanel.CurrentTurnHero.CurrentAction = Action;
    }
    
    protected void OnEnterActionButton(PointerEventData data)
    {
        BattleActionPanel.ShowActionInfo(Action);
    }
}