using UnityEngine;

public sealed class BattleTile : AreaEventTile
{
    protected override void InitColor()
    {
        _outlineColor = _outline.color;
        _fillColor = _fill.color;
        _outlineHighlightColor = new Color(1f, 0.08f, 0.08f, 1f);
        _fillHighlightColor = new Color(1f, 0, 0, 0.67f);
    }

    public override void OnTileEnter()
    {   
        if(_willBeDestroyed) return; // 타일이 파괴될 예정이라면 이벤트를 실행하지 않음
        Managers.AreaMng.LoadBattleScene();
    }

    public override void OnTileEventFinish()
    {

    }
}
