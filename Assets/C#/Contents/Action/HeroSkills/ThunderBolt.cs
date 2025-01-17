public class ThunderBolt : JumpAttackAction
{
    public override void SetInfo(int dataId)
    {
        ActionTargetType = Define.ActionTargetType.Single;
        
        base.SetInfo(dataId);
    }
}
