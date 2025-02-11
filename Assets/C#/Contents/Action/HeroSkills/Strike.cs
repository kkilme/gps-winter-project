using UnityEngine;

public class Strike : MoveAttackAction
{
    public override void SetInfo(int dataId)
    {
        ActionTargetType = ActionTargetType.Single;
        
        base.SetInfo(dataId);
    }
}
