public class ThunderBolt : StandAction
{
    public override void SetInfo(int dataId)
    {
        ActionTargetType = ActionTargetType.Single;
        
        base.SetInfo(dataId);
    }
}
