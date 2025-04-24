using UnityEngine;

public sealed class NormalTile : AreaEventTile
{
    public override void OnTileEnter()
    {
        CoroutineRunner.Instance.StartCoroutine(Managers.AreaMng.OnTileEventFinish());
    }

    public override void OnTileEventFinish()
    {
        
    }
}
