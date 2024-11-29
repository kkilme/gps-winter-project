using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public sealed class EncounterTile : AreaGridTile
{
    private const string _iconPath = "Area/icon_encounter";

    public EncounterTile(Vector3 position, GameObject tileObject = null) : base(position, tileObject)
    {
        TileType = AreaTileType.Encounter;
        _indicatorColor = new Color(255f / 255f, 255f / 255f, 20f / 255f, 200f / 255f);
        _fillColor = new Color(255f / 255f, 255f / 255f, 0f / 255f, 50f / 255f);
        _indicatorHighlightColor = new Color(255f / 255f, 255f / 255f, 20f / 255f, 255f / 255f);
        _fillHighlightColor = new Color(255f / 255f, 255f / 255f, 0f / 255f, 150f / 255f);
        Init();
    }

    public override void Init()
    {   
       _indicator.color = _indicatorColor;
       _fill.color = _fillColor;

        Icon = Managers.ResourceMng.Instantiate(_iconPath, TileObject.transform, "icon");
    }
    public override void OnTileEnter()
    {
        Managers.SceneMng.GetCurrentScene<AreaScene>().AreaState = AreaState.Idle; // TODO - Encounter 구현 시 상태 수정
    }

    public override void OnTileEventFinish()
    {
        throw new System.NotImplementedException();
    }
}
