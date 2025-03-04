using System.Collections.Generic;
using UnityEngine;

public class AllySelector : ActionTargetSelector
{
    public override bool NeedTargetSelection { get; protected set; } = true;
    public override void SetTargettableCells()
    {
        //TODO
    }
}
