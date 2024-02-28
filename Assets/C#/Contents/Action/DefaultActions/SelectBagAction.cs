using UnityEngine;

public class SelectBagAction : BaseAction
{
    public void SetInfo(Creature owner)
    {
        Owner = owner;
            
        ActionAttribute = Define.ActionAttribute.SelectBag;
    }

    public override void HandleAction(BattleGridCell cell)
    {
        
    }
}