using UnityEngine;

public sealed class NormalTile : AreaEventTile
{
    public override void OnTileEnter()
    {
        if (_willBeDestroyed) return;
        CoroutineRunner.Instance.StartCoroutine(Managers.AreaMng.OnTileEventFinish());
    }
}
