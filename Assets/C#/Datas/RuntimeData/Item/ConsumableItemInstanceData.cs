using System.Collections;
using UnityEngine;


public class ConsumableItemInstanceData : ItemInstanceData
{
    public ConsumableItemData ConsumableItemData => ItemData as ConsumableItemData;
    public ConsumableItemInstanceData(int itemDataId, int instanceId, int quantity = 1): base(itemDataId, instanceId) 
    {
        Quantity = quantity;
    }
}
