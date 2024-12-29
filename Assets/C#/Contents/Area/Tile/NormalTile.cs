using UnityEngine;

public sealed class NormalTile : AreaEventTile
{
    public override void OnTileEnter()
    {
        Managers.AreaMng.OnTileEventFinish();   
    }

    public override void OnTileEventFinish()
    {
        
    }
}
