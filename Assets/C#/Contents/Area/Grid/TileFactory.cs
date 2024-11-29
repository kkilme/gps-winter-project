using System;
using UnityEngine;
using static Define;
public static class TileFactory
{
    public static AreaGridTile CreateTile(Vector3 position, AreaTileType type, GameObject tileObject = null)
    {
        switch (type)
        {
            case AreaTileType.Normal:
                return new NormalTile(position, tileObject);
            case AreaTileType.Battle:
                return new BattleTile(position, tileObject);
            case AreaTileType.Encounter:
                return new EncounterTile(position, tileObject);
            case AreaTileType.Boss:
                return new BossTile(position, tileObject);
            case AreaTileType.Destroyed:
                return new DestroyedTile(position, tileObject);
            default:
                return new NormalTile(position, tileObject);
        }
    }
}
