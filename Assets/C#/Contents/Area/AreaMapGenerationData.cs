using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Area Map Data", menuName = "Scriptable Object/Area Map Data")]
public class AreaMapGenerationData : ScriptableObject
{
    // 맵 원점
    public Vector3 OriginPosition;
    // 전체 맵 크기
    public int MapWidth;
    public int MapHeight;
    // 플레이 가능(이동 가능)한 영역 크기
    [Tooltip("플레이 가능한 영역의 가로 크기, Note: 반드시 홀수여야 함")]
    public int PlayableFieldWidth;
    public int PlayableFieldHeight;
    [Range(0, 1), Tooltip("플레이 불가능한 필드에서의 장식물 비율")]
    public float UnplayableFieldDecorationProportion;
    [Range(0, 0.8f), Tooltip("플레이 가능한 필드에서의 장식물 비율")]
    public float PlayableFieldDecorationProportion;

    public int BattleTileNum;
    public int EncounterTileNum;
    public AreaTileGroupData MainTileGroupData;
    public AreaSubTileGroupData[] SubTileGroupData;

    // 데이터 오류 검증
    public bool Verify()
    {
        if (MainTileGroupData == null)
        {
            Debug.LogError("MainTileGroupData is null!");
            return false;
        }

        if (SubTileGroupData == null || SubTileGroupData.Length == 0)
        {
            Debug.LogError("SubTileGroupData is null or empty!");
            return false;
        }

        if (MapHeight <= PlayableFieldHeight || MapWidth <= PlayableFieldWidth)
        {
            Debug.LogError("PlayableMapSize must be smaller than MapSize!");
            return false;
        }

        if (BattleTileNum > PlayableFieldHeight * PlayableFieldWidth / 2 || EncounterTileNum > PlayableFieldHeight * PlayableFieldWidth / 2)
        {
            Debug.LogError("EventTileNum must be smaller than (PlayableFieldHeight * PlayableFieldWidth) / 2!");
            return false;
        }

        if (PlayableFieldWidth % 2 == 0)
        {
            Debug.LogError("PlayableFieldWidth must be an odd number!");
            return false;
        }

        foreach (var subTileGroup in SubTileGroupData)
        {
            if (subTileGroup.MinLength == 0)
            {
                Debug.LogError("SubTileGroup's MinLength must not be 0!");
                return false;
            }

            if (subTileGroup.MaxLength < subTileGroup.MinLength)
            {
                Debug.LogError("SubTileGroup's MaxLength must be greater than MinLength!");
                return false;
            }

            if (subTileGroup.MaxLength - subTileGroup.MinLength == 0)
            {
                Debug.LogError("SubTileGroup's MaxLength and MinLength must not be the same!");
                return false;
            }
        }

        return true;
    }
}

[Serializable]
public class AreaTileGroupData
{
    public AreaBaseTileData[] Tiles;
    private List<AreaBaseTileData> _globalAvailableTiles;
    protected Dictionary<string, int> _globalCount = new();


    public void Init()
    {
        _globalCount = new();
        _globalAvailableTiles = Tiles.ToList();
    }

    // 타일 목록 중 랜덤으로 1개 타일 선택. 땅 종류는 같지만 땅 위에 배치되어있는 장식물이 다름.
    public AreaBaseTileData SelectRandomTile()
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
public class AreaSubTileGroupData
{
    [Range(0, 1), Tooltip("이 SubTile이 전체 맵에서 차지할 비율")]
    public float Proportion;

    public AreaSubTileData[] Tiles;
    private List<AreaSubTileData> _globalAvailableTiles;
    protected Dictionary<string, int> _globalCount = new();

    // 한 타일 그룹의 최소, 최대 크기
    public int MinLength;
    public int MaxLength;

    // 한 타일 그룹 내에서의 사용 가능한 타일들
    private List<AreaSubTileData> _localAvailableTiles;
    private Dictionary<string, int> _localCount = new();

    public void Init()
    {
        _globalCount = new();
        _globalAvailableTiles = Tiles.ToList();
        _localCount = new();
        _localAvailableTiles = Tiles.ToList();
    }

    public void OnNextTilegroupStart()
    {
        _localCount = new();
        _localAvailableTiles = new List<AreaSubTileData>(_globalAvailableTiles);
    }

    public AreaSubTileData SelectRandomTile()
    {
        if (_localAvailableTiles.Count == 0)
        {
            Debug.LogError("No more selectable tile left!");
        }

        AreaSubTileData tileData = _localAvailableTiles[Random.Range(0, _localAvailableTiles.Count)];

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
    [Header("Only applied when HasLimit is True")]
    [Tooltip("전체 맵에서 이 타일이 생성될 수 있는 최대 개수")]
    public int GlobalLimitCount;
}

[Serializable]
public class AreaSubTileData
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