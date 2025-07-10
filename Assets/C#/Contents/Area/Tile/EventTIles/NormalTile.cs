using UnityEngine;

public sealed class NormalTile : AreaEventTile
{
    protected override void InitColor()
    {
        _outlineColor = _outline.color;
        _fillColor = _fill.color;
        _outlineHighlightColor = new Color(0.2f, 1f, 0, 1f);
        _fillHighlightColor = new Color(0.2f, 1f, 0, 0.25f);
    }

    public override void OnTileEnter()
    {
        if (_willBeDestroyed) return;
        CoroutineRunner.Instance.StartCoroutine(Managers.AreaMng.OnTileEventFinish());
    }
}
