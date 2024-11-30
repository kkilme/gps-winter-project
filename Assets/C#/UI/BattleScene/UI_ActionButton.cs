using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ActionButton : UI_Base
{
    public Button Button { get; protected set; }
    
    public UI_BattleOrder UIBattleOrder { get; set; }
    
    public BaseAction Action { get; set; }
    
    public override void Init()
    {
        Button = gameObject.GetOrAddComponent<Button>();
        
        Button.onClick.AddListener(OnClickActionButton);
        gameObject.BindEvent(OnEnterActionButton, Define.UIEvent.Enter);
    }

    protected void OnClickActionButton()
    {
        UIBattleOrder.CurrentTurnHero.CurrentAction = Action;
    }
    
    protected void OnEnterActionButton(PointerEventData data)
    {
        UIBattleOrder.ShowActionInfo(Action);
    }
}