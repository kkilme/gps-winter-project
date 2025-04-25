using DG.Tweening;
using UnityEngine;

public sealed class CollapsedTile : AreaEventTile
{
    protected override void InitColor()
    {
        _outlineColor = _outline.color;
        _fillColor = _fill.color;
        _outlineHighlightColor = new Color(1f, 0f, 0f, 1f);
        _fillHighlightColor = new Color(0.6f, 0, 0, 1f);
    }

    public override void OnTileEnter()
    {
        if (_willBeDestroyed) return;
        CoroutineRunner.Instance.StartCoroutine(Managers.AreaMng.OnTileEventFinish());
    }

    public override void OnTileEventFinish()
    {
        
    }
}
