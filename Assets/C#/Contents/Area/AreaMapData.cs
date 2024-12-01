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
    // 전체 맵 크기
    public int MapWidth;
    public int MapHeight;
    // 플레이 가능(이동 가능)한 맵 크기
    public int PlayableMapWidth;
    public int PlayableMapHeight;
    [Range(0, 1), Tooltip("플레이 불가능한 필드에서의 장식물 비율")]
    public float UnplayableFieldDecorationProportion;
    [Range(0, 0.8f), Tooltip("플레이 가능한 필드에서의 장식물 비율")]
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
    protected List<AreaBaseTileData> _globalAvailableTiles;
    protected Dictionary<string, int> _globalCount = new();


    public virtual void Init()
    {
        _globalCount = new();
        _globalAvailableTiles = Tiles.ToList();
    }

    // 타일 목록 중 랜덤으로 1개 타일 선택. 땅 종류는 같지만 땅 위에 배치되어있는 장식물이 다름.
    public virtual AreaBaseTileData SelectRandomTile()
    {
        if (_globalAvailableTiles.Count == 0)
        {
            Debug.LogError("No more selectable tile left!");
        }

        AreaBaseTileData tileData = _globalAvailableTiles[Random.Range(0, _globalAvailableTiles.Count)];

        Util.IncreaseDictCount(_globalCount, tileData.Name);
        
        // GlobalLimit에 도달 시
        if (tileData.HasGlobalLimit && _globalCount[tileData.Name] == tileData.GlobalLimitCount)
        {
            _globalAvailableTiles.Remove(tileData);
        }

        return tileData;
    }
}

[Serializable]
public class AreaSubTileGroupData : AreaTileGroupData
{
    [Range(0, 1), Tooltip("이 SubTile이 전체 맵에서 차지할 비율")]
    public float Proportion;

    // 한 타일 그룹의 최소, 최대 크기
    public int MinLength;
    public int MaxLength;
    // 한 타일 그룹 내에서의 사용 가능한 타일들
    private List<AreaBaseTileData> _localAvailableTiles;
    private Dictionary<string, int> _localCount = new();

    public override void Init()
    {
        base.Init();

        _localCount = new();
        _localAvailableTiles = Tiles.ToList();
    }

    public void OnNextTilegroupStart()
    {
        _localCount = new();
        _localAvailableTiles = new List<AreaBaseTileData>(_globalAvailableTiles);
    }

    public override AreaBaseTileData SelectRandomTile()
    {
        if (_localAvailableTiles.Count == 0)
        {
            Debug.LogError("No more selectable tile left!");
        }

        AreaBaseTileData tileData = _localAvailableTiles[Random.Range(0, _localAvailableTiles.Count)];

        Util.IncreaseDictCount(_globalCount, tileData.Name);
        Util.IncreaseDictCount(_localCount, tileData.Name);

        // GlobalLimit에 도달 시
        if (tileData.HasGlobalLimit && _globalCount[tileData.Name] == tileData.GlobalLimitCount)
        {
            _globalAvailableTiles.Remove(tileData);
            _localAvailableTiles.Remove(tileData);
        }
        // LocalLimit에 도달 시
        if (tileData.HasLocalLimit && _localCount[tileData.Name] == tileData.LocalLimitCount)
        {
            _localAvailableTiles.Remove(tileData);
        }

        return tileData;
    }
}

[Serializable]
public class AreaBaseTileData
{
    public GameObject Tile;
    public string Name => Tile.name;
    public bool HasGlobalLimit;
    public bool HasLocalLimit;

    [Header("Only applied when HasLimit is True")]
    [Tooltip("전체 맵에서 이 타일이 생성될 수 있는 최대 개수")]
    public int GlobalLimitCount;
    [Tooltip("하나의 TileGroup에서 이 타일이 생성될 수 있는 최대 개수")]
    public int LocalLimitCount;
}