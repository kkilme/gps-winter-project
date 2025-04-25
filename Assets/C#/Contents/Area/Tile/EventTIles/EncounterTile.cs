using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public sealed class EncounterTile : AreaEventTile
{
    protected override void InitColor()
    {
        _outlineColor = _outline.color;
        _fillColor = _fill.color;
        _outlineHighlightColor = new Color(1f, 1f, 0.08f, 1f);
        _fillHighlightColor = new Color(1f, 1f, 0, 0.6f);
    }

    public override void OnTileEnter()
    {
        if (_willBeDestroyed) return;
        Managers.AreaMng.AreaState = AreaState.Idle; // TODO - Encounter 구현 시 상태 수정
    }

    public override void OnTileEventFinish()
    {
        throw new System.NotImplementedException();
    }
}
