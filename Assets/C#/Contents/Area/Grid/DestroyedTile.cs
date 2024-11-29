using DG.Tweening;
using UnityEngine;
using static Define;

public sealed class DestroyedTile : AreaGridTile
{
    private const string _iconPath = "Area/icon_caution";

    public DestroyedTile(Vector3 position, GameObject tileObject = null) : base(position, tileObject)
    {
        TileType = AreaTileType.Destroyed;
        _indicatorColor = new Color(255f / 255f, 0 / 255f, 0 / 255f, 48f / 255f);
        _fillColor = new Color(0 / 255f, 0f / 255f, 0f / 255f, 223f / 255f);
        _indicatorHighlightColor = new Color(255f / 255f, 20f / 255f, 20f / 255f, 255f / 255f);
        _fillHighlightColor = new Color(255f / 255f, 0f / 255f, 0f / 255f, 170f / 255f);
        Init();
    }

    public override void Init()
    {   
        ChangeColor(TileColorChangeType.Reset);
        Icon = Managers.ResourceMng.Instantiate(_iconPath, TileObject.transform, "icon");
    }

    public override void OnTileEnter()
    {   
        // TODO: 정예몹 배틀
        Managers.AreaMng.AreaState = AreaState.Battle;
        Managers.SceneMng.GetCurrentScene<AreaScene>().LoadBattleScene();
    }

    public override void OnTileEventFinish()
    {
        
    }
}
