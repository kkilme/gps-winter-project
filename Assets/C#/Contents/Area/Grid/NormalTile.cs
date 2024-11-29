using UnityEngine;
using static Define;

public sealed class NormalTile : AreaGridTile
{
    public NormalTile(Vector3 position, GameObject tileObject = null) : base(position, tileObject)
    {
        TileType = AreaTileType.Normal;
        _indicatorColor = _indicator.color;
        _fillColor = _fill.color;
        _indicatorHighlightColor = new Color(50f / 255f, 1f, 0);
        _fillHighlightColor = new Color(50f / 255f, 1f, 0, 65f / 255f);

        Init();
    }

    public override void Init()
    {
        return;
    }
    public override void OnTileEnter()
    {
        Managers.AreaMng.OnTileEventFinish();   
    }

    public override void OnTileEventFinish()
    {
        
    }
}
