using System.Collections;
using UnityEngine;


public class ConsumableItemInstanceData : ItemInstanceData
{
    public ConsumableItemData ConsumableItemData => ItemData as ConsumableItemData;
    public int Quantity { get; set; } // 사용 가능한 수량, 기본값은 1
    public ConsumableItemInstanceData(int itemDataId, int instanceId, int quantity = 1): base(itemDataId, instanceId) 
    {
        Quantity = quantity;
    }
}
