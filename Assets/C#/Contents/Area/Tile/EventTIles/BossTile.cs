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
        if (_willBeDestroyed) return; // 타일이 파괴될 예정이라면 이벤트를 실행하지 않음
        Managers.AreaMng.LoadBattleScene(BattleType.Boss);
    }
}
