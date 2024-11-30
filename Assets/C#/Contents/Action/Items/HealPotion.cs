public class HealPotion : BaseItem
{
    public Creature Target { get; protected set; }
    
    public override void SetInfo(int templateId, Creature owner, Bag bag, int idx, int addNum)
    {
        ActionTargetType = Define.ActionTargetType.Single;
        
        base.SetInfo(templateId, owner, bag, idx, addNum);
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
        if (TargetCell.CellCreature == null)
            return;
        
        Creature targetCreature = TargetCell.CellCreature;
        targetCreature.OnHeal(ItemData.Heal);
        
        base.OnHandleAction();
    }
}
