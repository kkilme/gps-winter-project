using System;
using UnityEngine;
using static Define;
public static class TileFactory
{
    private const string GRID_TILE_PATH = "Area/grid_hex";

    public static AreaEventTile CreateTile(Vector3 position, AreaTileType type, GameObject tileObject = null)
    {
        bool isRecycle = tileObject != null;

        if (!isRecycle)
        {
            Transform tileParent = GameObject.Find("@EventTiles").transform;
            tileObject = Managers.ResourceMng.Instantiate(GRID_TILE_PATH, tileParent);
            tileObject.transform.position = position;
        }

        AreaEventTile tile = type switch
        {
            AreaTileType.Normal => new NormalTile(position, tileObject, isRecycle),
            AreaTileType.Battle => new BattleTile(position, tileObject, isRecycle),
            AreaTileType.Encounter => new EncounterTile(position, tileObject, isRecycle),
            AreaTileType.Boss => new BossTile(position, tileObject, isRecycle),
            AreaTileType.Destroyed => new DestroyedTile(position, tileObject, isRecycle),
            _ => new NormalTile(position, tileObject, isRecycle),
        };
        return tile;
    }
}
