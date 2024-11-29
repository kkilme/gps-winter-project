using UnityEngine;
using static Define;

public sealed class BossTile : AreaGridTile
{
    private const string _iconPath = "Area/icon_boss";

    public BossTile(Vector3 position, GameObject tileObject = null) : base(position, tileObject)
    {
        TileType = AreaTileType.Boss;
        _indicatorColor = new Color(138 / 255f, 0 / 255f, 255 / 255f, 255 / 255f);
        _fillColor = new Color(85 / 255f, 0 / 255f, 163 / 255f, 222 / 255f);
        _indicatorHighlightColor = new Color(200f / 255f, 0f / 255f, 0 / 255f, 255 / 255f);
        _fillHighlightColor = new Color(163 / 255f, 0 / 255f, 0 / 255f, 222 / 255f);
        Init();
    }

    public override void Init()
    {   
       _indicator.color = _indicatorColor;
       _fill.color = _fillColor;

        Icon = Managers.ResourceMng.Instantiate(_iconPath, TileObject.transform, "icon");
        Icon.transform.position += new Vector3(0, 0, -0.1f);
    }
    public override void OnTileEnter()
    {
        Managers.SceneMng.GetCurrentScene<AreaScene>().AreaState = AreaState.Idle; // TODO - Boss 구현 시 상태 수정
    }

    public override void OnTileEventFinish()
    {
        throw new System.NotImplementedException();
    }
}
