using System;
using UnityEngine;
using static Define;
public static class TileFactory
{
    private const string NORMAL_TILE_PATH = "Area/Tiles/NormalTile";
    private const string BATTLE_TILE_PATH = "Area/Tiles/BattleTile";
    private const string ENCOUNTER_TILE_PATH = "Area/Tiles/EncounterTile";
    private const string BOSS_TILE_PATH = "Area/Tiles/BossTile";
    private const string DESTROYED_TILE_PATH = "Area/Tiles/DestroyedTile";

    public static AreaEventTile CreateTile(Vector3 position, AreaTileType type)
    {
        Transform tileParent = GameObject.Find("@EventTiles").transform;

        GameObject tile = type switch
        {
            AreaTileType.Normal => Managers.ResourceMng.Instantiate(NORMAL_TILE_PATH, tileParent),
            AreaTileType.Battle => Managers.ResourceMng.Instantiate(BATTLE_TILE_PATH, tileParent),
            AreaTileType.Encounter => Managers.ResourceMng.Instantiate(ENCOUNTER_TILE_PATH, tileParent),
            AreaTileType.Boss => Managers.ResourceMng.Instantiate(BOSS_TILE_PATH, tileParent),
            AreaTileType.Destroyed => Managers.ResourceMng.Instantiate(DESTROYED_TILE_PATH, tileParent),
            _ => Managers.ResourceMng.Instantiate(NORMAL_TILE_PATH, tileParent),
        };

        tile.transform.position = position;

        AreaEventTile areaEventTile = tile.GetComponent<AreaEventTile>();
        areaEventTile.Init();

        return areaEventTile;
    }
}
