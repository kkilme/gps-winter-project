using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class ConsumableItem : Item
{
    public ConsumableItemData ConsumableItemData => ItemData as ConsumableItemData;

    public ConsumableItem(int dataId)
    {
        SetData(dataId);
    }
}
