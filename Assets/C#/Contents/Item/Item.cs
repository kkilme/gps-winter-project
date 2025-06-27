using System.Collections;
using UnityEngine;

public class Item
{
    public int DataId { get; protected set; }
    public ItemData ItemData => Managers.DataMng.ItemDataDict[DataId];
    public ItemType ItemType => ItemData.ItemType;

    public virtual void SetData(int dataId)
    {
        DataId = dataId;
    }
}
