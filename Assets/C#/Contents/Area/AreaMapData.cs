using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[Serializable]
public class AreaMapData
{
    public int MapWidth;
    public int MapHeight;
    public int PlayableMapWidth;
    public int PlayableMapHeight;
    [Range(0, 1), Tooltip("Proportion of decoration tiles in unplayable field")]
    public float UnplayableFieldDecorationProportion;
    [Range(0, 0.8f), Tooltip("Proportion of decoration tiles in playable field")]
    public float PlayableFieldDecorationProportion;

    public int BattleTileNum;
    public int EncounterTileNum;
    public AreaTileGroupData MainTileGroupData;
    public AreaSubTileGroupData[] SubTileGroupData;
}

[Serializable]
public class AreaTileGroupData
{
    public AreaBaseTileData[] Tiles;
    public int MinLength;
    public int MaxLength;
    private List<AreaBaseTileData> _globalAvailableTiles;
    private List<AreaBaseTileData> _localAvailableTiles;
    private Dictionary<string, int> _globalCount = new();
    private Dictionary<string, int> _localCount = new();

    public void Init()
    {
        _globalCount = new();
        _localCount = new();
        _globalAvailableTiles = Tiles.ToList();
        _localAvailableTiles = Tiles.ToList();
    }

    public AreaBaseTileData SelectRandomTile()
    {
        if (_localAvailableTiles.Count == 0)
        {
            Debug.LogError("No more selectable tile left!");
        }

        AreaBaseTileData tileData = _localAvailableTiles[Random.Range(0, _localAvailableTiles.Count)];

        Util.IncreaseDictCount(_globalCount, tileData.Name);
        Util.IncreaseDictCount(_localCount, tileData.Name);
        
        if (tileData.HasGlobalLimit && _globalCount[tileData.Name] == tileData.GlobalLimitCount)
        {
            _globalAvailableTiles.Remove(tileData);
            _localAvailableTiles.Remove(tileData);
        }
        if (tileData.HasLocalLimit && _localCount[tileData.Name] == tileData.LocalLimitCount)
        {
            _localAvailableTiles.Remove(tileData);
        }

        return tileData;
    }

    public void OnNextTilegroupStart()
    {
        _localCount = new();
        _localAvailableTiles = new List<AreaBaseTileData>(_globalAvailableTiles);
    }
}

[Serializable]
public class AreaSubTileGroupData : AreaTileGroupData
{
    [Range(0, 1), Tooltip("Proportion of this subtile in the map")]
    public float Proportion;
}

[Serializable]
public class AreaBaseTileData
{
    public GameObject Tile;
    public string Name => Tile.name;
    public bool HasGlobalLimit;
    public bool HasLocalLimit;

    [Header("Only applied when HasLimit is True")]
    public int GlobalLimitCount;
    public int LocalLimitCount;
}