public abstract class BaseItem : BaseAction
{
    public int DataId { get; protected set; }
    public Data.ItemData ItemData { get; protected set; }
    public Define.ItemType ItemType { get; protected set; }
    
    public Bag Bag { get; set; }
    public int Idx { get; set; }
    public int Count { get; set; }

    public virtual void SetInfo(int templateId, Creature owner, Bag bag, int idx, int addNum)
    {
        DataId = templateId;
        ItemData = Managers.DataMng.ItemDataDict[templateId];
        
        Owner = owner;
        Bag = bag;
        Idx = idx;
        Count += addNum;
    }
    
    public override void OnHandleAction()
    {
        Count--;
        if (Count <= 0)
            Bag.Items[Idx] = null;
    }
}