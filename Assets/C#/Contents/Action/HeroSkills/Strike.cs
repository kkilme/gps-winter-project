using UnityEngine;

public class Strike : JumpAttackAction
{
    public override void SetInfo(int dataId)
    {
        ActionTargetType = Define.ActionTargetType.Single;
        
        base.SetInfo(dataId);
    }
}
