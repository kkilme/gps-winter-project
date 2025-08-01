using DG.Tweening;
using UnityEngine;

public sealed class CollapsedTile : AreaEventTile
{
    public override void OnTileEnter()
    {
        if (_willBeDestroyed) return;
        CoroutineRunner.Instance.StartCoroutine(Managers.AreaMng.OnTileEventFinish());
    }
}
