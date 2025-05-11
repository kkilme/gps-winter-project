using System.Collections;
using UnityEngine;

public class Item
{
    public int DataId { get; protected set; }
    public ItemData ItemData { get; protected set; }
    public ItemType ItemType { get; protected set; }

    public virtual void SetData(int dataId)
    {
        DataId = dataId;
    }
}
