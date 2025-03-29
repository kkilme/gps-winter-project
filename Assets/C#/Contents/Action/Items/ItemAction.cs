using System.Collections;
using Data;

// TODO: ItemAction (재작성 필요)
public abstract class ItemAction : BaseAction
{
    public ItemData ItemData { get; protected set; }
    public ItemType ItemType { get; protected set; }
    
    public Bag Bag { get; set; }
    public int Idx { get; set; }
    public int Count { get; set; }

    public virtual void SetInfo(int dataId, Creature owner, int idx, int addNum)
    {
        DataId = dataId;
        ItemData = Managers.DataMng.ItemDataDict[dataId];
        
        Executor = owner;
        Idx = idx;
        Count += addNum;
    }
    
    public override IEnumerator Execute()
    {
        Count--;
        if (Count <= 0)
            Bag.Items[Idx] = null;
        yield return null;
    }
}