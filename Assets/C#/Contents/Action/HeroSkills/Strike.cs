using UnityEngine;

public class Strike : MoveAttackAction
{
    public override void SetInfo(int dataId)
    {
        ActionTargetType = GlobalEnums.ActionTargetType.Single;
        
        base.SetInfo(dataId);
    }
}
