using System;
using UnityEngine;

// 타입을 받아 Area 타일(AreaEventTile) 및 FogOfWar을 생성하는 팩토리 클래스
public static class AreaTileFactory
{
    private const string NORMAL_TILE_PATH = "Area/EventTiles/NormalTile";
    private const string BATTLE_TILE_PATH = "Area/EventTiles/BattleTile";
    private const string ENCOUNTER_TILE_PATH = "Area/EventTiles/EncounterTile";
    private const string BOSS_TILE_PATH = "Area/EventTiles/BossTile";
    private const string COLLAPSED_TILE_PATH = "Area/EventTiles/CollapsedTile";

    private const string FOW_PLAYABLE_FIELD_PATH = "Area/EventTiles/FogOfWar_PlayableField";
    private const string FOW_UNPLAYABLE_FIELD_PATH = "Area/EventTiles/FogOfWar_UnplayableField";

    public static AreaEventTile CreateTile(Vector3 position, AreaTileType type, Transform parent)
    {
        GameObject tile = type switch
        {
            AreaTileType.Normal => Managers.ResourceMng.Instantiate(NORMAL_TILE_PATH, parent),
            AreaTileType.Battle => Managers.ResourceMng.Instantiate(BATTLE_TILE_PATH, parent),
            AreaTileType.Encounter => Managers.ResourceMng.Instantiate(ENCOUNTER_TILE_PATH, parent),
            AreaTileType.Boss => Managers.ResourceMng.Instantiate(BOSS_TILE_PATH, parent),
            AreaTileType.Collapsed => Managers.ResourceMng.Instantiate(COLLAPSED_TILE_PATH, parent),
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
            Managers.ResourceMng.Instantiate(FOW_UNPLAYABLE_FIELD_PATH, parent) : Managers.ResourceMng.Instantiate(FOW_PLAYABLE_FIELD_PATH, parent);

        fogOfWarObject.transform.position = position;

        FogOfWar fogOfWar = fogOfWarObject.GetComponent<FogOfWar>();

        return fogOfWar;
    }
}
