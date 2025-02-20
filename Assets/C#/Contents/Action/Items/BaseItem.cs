using System.Collections;
using Data;

// TODO: Item
public abstract class BaseItem : BaseAction
{
    public ItemData ItemData { get; protected set; }
    public ItemType ItemType { get; protected set; }
    
    public Bag Bag { get; set; }
    public int Idx { get; set; }
    public int Count { get; set; }

    public virtual void SetInfo(int dataId, Creature owner, Bag bag, int idx, int addNum)
    {
        DataId = dataId;
        ItemData = Managers.DataMng.ItemDataDict[dataId];
        
        Owner = owner;
        Bag = bag;
        Idx = idx;
        Count += addNum;
    }
    
    public override IEnumerator Execute(int coinHeadCount = -1)
    {
        Count--;
        if (Count <= 0)
            Bag.Items[Idx] = null;
        yield return null;
    }
}