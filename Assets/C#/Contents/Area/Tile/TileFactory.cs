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

    private const string FOG_OF_WAR_ON_PLAYABLE_FIELD_PATH = "Area/Tiles/FogOfWar_PlayableField";
    private const string FOG_OF_WAR_ON_UNPLAYABLE_FIELD_PATH = "Area/Tiles/FogOfWar_UnplayableField";

    public static AreaEventTile CreateTile(Vector3 position, AreaTileType type, Transform parent)
    {
        GameObject tile = type switch
        {
            AreaTileType.Normal => Managers.ResourceMng.Instantiate(NORMAL_TILE_PATH, parent),
            AreaTileType.Battle => Managers.ResourceMng.Instantiate(BATTLE_TILE_PATH, parent),
            AreaTileType.Encounter => Managers.ResourceMng.Instantiate(ENCOUNTER_TILE_PATH, parent),
            AreaTileType.Boss => Managers.ResourceMng.Instantiate(BOSS_TILE_PATH, parent),
            AreaTileType.Destroyed => Managers.ResourceMng.Instantiate(DESTROYED_TILE_PATH, parent),
            _ => Managers.ResourceMng.Instantiate(NORMAL_TILE_PATH, parent),
        };

        tile.transform.position = position;

        AreaEventTile areaEventTile = tile.GetComponent<AreaEventTile>();
        areaEventTile.Init();

        return areaEventTile;
    }

    public static FogOfWar CreateFogOfWar(Vector3 position, bool isUnplayableField, Transform parent)
    {
        GameObject fogOfWarObject = isUnplayableField ?  
            Managers.ResourceMng.Instantiate(FOG_OF_WAR_ON_UNPLAYABLE_FIELD_PATH, parent) : Managers.ResourceMng.Instantiate(FOG_OF_WAR_ON_PLAYABLE_FIELD_PATH, parent);

        fogOfWarObject.transform.position = position;

        FogOfWar fogOfWar = fogOfWarObject.GetComponent<FogOfWar>();

        return fogOfWar;
    }
}
