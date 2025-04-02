using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPurchasable
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int SellingPrice { get; }
    public int PurchasePrice { get; }
}
