using UnityEngine;

public class FleeAction : BaseAction
{
    public int CoinNum { get; set; }
    
    public override void SetInfo(int dataId)
    {
        base.SetInfo(dataId);
            
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