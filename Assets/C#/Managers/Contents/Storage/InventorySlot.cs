using System.Collections;
using UnityEngine;


public class InventorySlot
{
    public Item Item { get; private set; }
    public int Quantity { get; private set; }

    public InventorySlot(Item item, int quantity)
    {
        Item = item;
        Quantity = quantity;
    }

    public void AddQuantity(int quantity)
    {
        Quantity += quantity;
    }
}
