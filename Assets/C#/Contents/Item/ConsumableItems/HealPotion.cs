using System.Collections;
using UnityEngine;


public class HealPotion : ConsumableItem, IUsableInArea, IUsableInBattle
{
    public HealPotion(int dataId) : base(dataId) { }

    public ItemAction GetItemAction()
    {
        return new HealPotionAction();
    }

    public void UseInArea()
    {
        // Area에서 HealPotion 사용 로직 구현  
        Debug.Log("HealPotion used in area.");
    }
}