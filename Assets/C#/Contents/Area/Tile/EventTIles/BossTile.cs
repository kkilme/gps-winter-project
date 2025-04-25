using UnityEngine;

public sealed class BossTile : AreaEventTile
{
    protected override void InitColor()
    {
        _outlineColor = _outline.color;
        _fillColor = _fill.color;
        _outlineHighlightColor = new Color(0.78f, 0, 0, 1f);
        _fillHighlightColor = new Color(0.63f, 0, 0, 0.87f);
    }

    public override void OnTileEnter()
    {
        Managers.AreaMng.AreaState = AreaState.Idle; // TODO - Boss 구현 시 상태 수정
    }

    public override void OnTileEventFinish()
    {
        throw new System.NotImplementedException();
    }
}
