using UnityEngine;

public class FleeAction : BaseAction
{
    public int CoinNum { get; set; }
    
    public override void SetInfo(int templateId)
    {
        base.SetInfo(templateId);
            
        ActionTargetType = Define.ActionTargetType.Single;
    }

    public override bool CanStartAction()
    {
        // TODO
        return false;
    }

    public override void OnStartAction()
    {
        // TODO
    }
    
    public override void OnHandleAction()
    {
        // TODO
    }
}