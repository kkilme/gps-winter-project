public class Item
{
    public int DataId { get; protected set; }
    public ItemData ItemData => Managers.DataMng.ItemDataDict[DataId];

    public virtual void SetData(int dataId)
    {
        DataId = dataId;
    }
}
