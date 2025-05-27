using System.Collections;
using UnityEngine;


public class ItemInstanceData
{
    public int ItemDataId { get; protected set; } // ItemData의 DataId
    public int InstanceId { get; protected set; } // 아이템의 InstanceId
    public ItemData ItemData => Managers.DataMng.ItemDataDict[ItemDataId]; // 아이템 데이터
    public ItemType ItemType => ItemData.ItemType;

    public ItemInstanceData(int itemDataId, int instanceId)
    {
        ItemDataId = itemDataId;
        InstanceId = instanceId;
    }
}
