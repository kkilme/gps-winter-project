public class ThunderBolt : StandAction
{
    public override void SetInfo(int dataId)
    {
        ActionTargetType = GlobalEnums.ActionTargetType.Single;
        
        base.SetInfo(dataId);
    }
}
