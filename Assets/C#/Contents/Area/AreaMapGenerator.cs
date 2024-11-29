using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class AreaMapGenerator : MonoBehaviour
{
    [SerializeField] private Vector3 _mapOriginPosition;
    [SerializeField] private SerializedDictionary<Define.AreaName, AreaMapData> _dataset;

    private AreaMapData _data;
    private Define.AreaTileType[,] _map;
    private AreaBaseTile[,] _baseTiles;
    private AreaEventTile[,] _eventTiles; // 주의: 위 두 배열과 다르게 PlayableMapHeight/Width 크기임

    private Transform _tileParent;
    private Transform _debugObjectParent;
    private Light _light;

    [Header("Debug")]
    [SerializeField, Tooltip("테스트 용으로 생성할 Area")] private Define.AreaName _testAreaName;

    [SerializeField] private GameObject _infoText;
    [SerializeField] private GameObject _pathIndicator;
    [ReadOnly] public MapGeneratePhase CurrentGeneratePhase = MapGeneratePhase.NotStarted; // 에디터상에서 버튼으로 생성 테스트하는 데 사용

    public void Init(Define.AreaName area = Define.AreaName.Forest)
    {
        #if UNITY_EDITOR
        area = _testAreaName;
        ClearMap();
        #endif

        _data = _dataset[area];
        _baseTiles = new AreaBaseTile[_data.MapHeight, _data.MapWidth];
        _eventTiles = new AreaEventTile[_data.PlayableMapHeight, _data.PlayableMapWidth];
        _map = new Define.AreaTileType[_data.MapHeight, _data.MapWidth];

        for (int i = 0; i < _data.MapHeight; i++)
        {   
            for (int j = 0; j < _data.MapWidth; j++)
            {
                _map[i, j] = Define.AreaTileType.Empty;
            }
        }

        GameObject mapObject = GameObject.Find("@Map") ?? Managers.ResourceMng.Instantiate("Area/@Map");
        _debugObjectParent = GameObject.Find("@Debug").transform;
        _tileParent = GameObject.Find("@BaseTiles").transform;
        _light = GameObject.FindGameObjectWithTag("AreaLight").GetComponent<Light>();
        _playableAreaHeightStartOffset = _data.MapHeight / 2 - _data.PlayableMapHeight / 2;
        _playableAreaWidthStartOffset = _data.MapWidth / 2 - _data.PlayableMapWidth / 2;
    }

    public void GenerateMap()
    {
        GenerateSubtiles();
        GenerateMainTile();
        SetupPlayableField(out List<Vector2Int> playableField, out List<Vector2Int> unplayableField);
        GenerateUnplayableFieldDecoration(unplayableField);
        GeneratePlayableFieldDecoration(playableField);
        GenerateEventTiles();
    }

    public void GenerateSubtiles()
    {
        AreaSubTileGroupData[] subTileGroupDatas = _data.SubTileGroupData;
        if (subTileGroupDatas.Length == 0) return;

        if (subTileGroupDatas.Sum(x => x.Proportion) > 1)
        {
            Debug.LogError("Sum of subtiles' Proportion is over 1!");
            return;
        }

        CurrentGeneratePhase = MapGeneratePhase.SubtileGenerate;

        // 타일이 맵에 최대한 골고루 퍼지도록 하기위해 사용되는 배열: 각 행/열 별로 생성된 타일 수의 합
        // numOfsubtiles[i,j]: i행에 생성된 subtile 수 + j열에 생성된 subtile수
        int[,] numOfSubtiles = new int[_data.MapHeight, _data.MapWidth];
       
        foreach (AreaSubTileGroupData subTileGroupData in subTileGroupDatas)
        {
            subTileGroupData.Init();
            int totalGenerated = 0;

            // Subtile의 정해진 비율에 완벽히 맞추진 않으나 비슷하도록 타일 생성
            while ((float)(totalGenerated) / (_data.MapHeight * _data.MapWidth) < subTileGroupData.Proportion)
            {   
                // 생성될 Subtile 그룹의 길이
                int length = Random.Range(subTileGroupData.MinLength, subTileGroupData.MaxLength);

                // 행/열 합하여 가장 적은 수의 타일이 생성된 위치를 가져옴
                Util.FindMinIndex(numOfSubtiles, out int x, out int z);
                _nextTilePosition = new Vector2Int(x, z);

                Transform tileGroupParent = SetupTileGroupParent(x, z);

                for (int i = 0; i < length; i++)
                {
                    x = _nextTilePosition.x;
                    z = _nextTilePosition.y;
                    Vector3 worldPos = GridToWorldPosition(x, z);

                    // 랜덤한 타일 선택해서 가져옴
                    AreaBaseTileData tileData = subTileGroupData.SelectRandomTile();
                    AreaBaseTile tile = Instantiate(tileData.Tile, worldPos, Quaternion.identity, tileGroupParent).GetComponent<AreaBaseTile>();

                    _baseTiles[z, x] = tile;
                    _map[z,x] = Define.AreaTileType.SubTile;
                    totalGenerated++;
                    IncreaseNumOfSubtiles(x, z);

                    if (!SetNextTilePosition(x, z)) break;
                }
                subTileGroupData.OnNextTilegroupStart();
            }
        }
        return;

        void IncreaseNumOfSubtiles(int x, int z)
        {
            for (int i = 0; i < _data.MapHeight; i++)
            {
                numOfSubtiles[i, x]++;
            }
            for (int i = 0; i < _data.MapWidth; i++)
            {
                numOfSubtiles[z, i]++;
            }
        }
    }

    public void GenerateMainTile()
    {
        CurrentGeneratePhase = MapGeneratePhase.Maintilegenerate;
        AreaTileGroupData mainTileGroupData = _data.MainTileGroupData;
        List<Vector2Int> emptyPositions = new();
        for (int z = 0; z < _data.MapHeight; z++)
        {
            for (int x = 0; x < _data.MapWidth; x++)
            {
                if (_map[z, x] == Define.AreaTileType.Empty)
                {
                    emptyPositions.Add(new Vector2Int(x, z));
                }
            }
        }
        mainTileGroupData.Init();

        // 일정 수까지는 그룹으로 생성
        while (emptyPositions.Count > MAINTILE_GROUP_GENERATE_END_OFFSET)
        {
            int length = Random.Range(mainTileGroupData.MinLength, mainTileGroupData.MaxLength);

            // 빈 위치 중 랜덤으로 그룹 시작 위치 설정
            _nextTilePosition = emptyPositions[Random.Range(0, emptyPositions.Count)];

            Transform tileGroupParent = SetupTileGroupParent(_nextTilePosition.x, _nextTilePosition.y);

            for (int i = 0; i < length; i++)
            {
                int x = _nextTilePosition.x;
                int z = _nextTilePosition.y;
                Vector3 worldPos = GridToWorldPosition(x, z);

                AreaBaseTileData tileData = mainTileGroupData.SelectRandomTile();
                AreaBaseTile tile = Instantiate(tileData.Tile, worldPos, Quaternion.identity, tileGroupParent).GetComponent<AreaBaseTile>();
                _baseTiles[z, x] = tile;
                _map[z, x] = Define.AreaTileType.MainTile;
                emptyPositions.Remove(_nextTilePosition);
                if (!SetNextTilePosition(x, z)) break;
            }
            mainTileGroupData.OnNextTilegroupStart();
        }

        // 남은 빈 위치에 생성
        foreach (var pos in emptyPositions)
        {
            int x = pos.x;
            int z = pos.y;
            Vector3 worldPos = GridToWorldPosition(x, z);

            AreaBaseTileData tileData = mainTileGroupData.SelectRandomTile();
            AreaBaseTile tile = Instantiate(tileData.Tile, worldPos, Quaternion.identity, _tileParent).GetComponent<AreaBaseTile>();

            _baseTiles[z, x] = tile;
            _map[z, x] = Define.AreaTileType.MainTile;
        }
    }

    public void SetupPlayableField(out List<Vector2Int> playableField, out List<Vector2Int> unplayableField)
    {
        InitBaseTiles();

        CurrentGeneratePhase = MapGeneratePhase.PlayableFieldSetup;
        _light.cullingMask = LayerMask.GetMask(_lightCullingMask);
  
        var tempPlayablePos = new List<Vector2Int>();
        unplayableField = new List<Vector2Int>();
        
        int xStart = _data.MapWidth / 2;
        int zStartOffset = 0, zEndOffset = 0;

        // 육각형꼴의 플레이 가능 필드 생성(설정)
        for (int xOffset = 0; xOffset <= _data.PlayableMapWidth / 2; xOffset++)
        {
            for (int z = _playableAreaHeightStartOffset + zStartOffset;
                 z < _playableAreaHeightStartOffset + _data.PlayableMapHeight - zEndOffset;
                 z++)
            {   
                SetPlayableTile(xStart + xOffset, z);
                SetPlayableTile(xStart - xOffset, z);
            }

            if ((xStart % 2 == 0 && xOffset % 2 == 0) || (xStart % 2 == 1 && xOffset % 2 == 1))
            {
                zEndOffset++;
            }
            else
            {
                zStartOffset++;
            }
        }

        // 플레이 불가능한 필드의 좌표를 리스트에 추가
        for (int z = 0; z < _data.MapHeight; z++)
        {
            for (int x = 0; x < _data.MapWidth; x++)
            {
                if (_map[z, x] == Define.AreaTileType.Empty)
                    continue;
                unplayableField.Add(new Vector2Int(x, z));
                _map[z, x] = Define.AreaTileType.OutOfField;
            }
        }

        // 시작 타일 및 보스 타일과 그 이웃 타일엔 장애물 배치 안함
        _playerStartPosition = new Vector2Int(xStart, _playableAreaHeightStartOffset);
        _bossPosition = new Vector2Int(xStart, _playableAreaHeightStartOffset + _data.PlayableMapHeight - 1);
        List<Vector2Int> notDecoratableTiles = new() { _playerStartPosition, _bossPosition };
        notDecoratableTiles.AddRange(GetNeighbors(_playerStartPosition));
        notDecoratableTiles.AddRange(GetNeighbors(_bossPosition));

        foreach (var pos in notDecoratableTiles)
        {
            _map[pos.y, pos.x] = Define.AreaTileType.ForceEmpty;
        }

        playableField = tempPlayablePos;
        return;

        void SetPlayableTile(int x, int z)
        {
            _baseTiles[z, x].SetLightTargetLayer();
            _map[z, x] = Define.AreaTileType.Empty;
            tempPlayablePos.Add(new Vector2Int(x, z));
        }
    }

    public void GenerateUnplayableFieldDecoration(List<Vector2Int> unplayableField)
    {
        CurrentGeneratePhase = MapGeneratePhase.UnplayableFieldDecorationGenerate;
        GenerateObstacles(unplayableField, _data.UnplayableFieldDecorationProportion);
    }

    public void GeneratePlayableFieldDecoration(List<Vector2Int> playableField)
    {
        CurrentGeneratePhase = MapGeneratePhase.PlayableFieldDecorationGenerate;

        List<Vector2Int> field;
        List<Vector2Int> path;
        float proportion = _data.PlayableFieldDecorationProportion;
        int trycount = 0;

        do
        {   
            Reset();
            GenerateObstacles(field, proportion);
            trycount++;
            if (trycount % 100 == 0) proportion -= 0.05f;
        } while (!FindPath(_bossPosition, out path) && proportion >= 0);

        Debug.Log($"TryCount: {trycount}, Obstacle Proportion: {proportion}, Path Length: {path.Count}");

        return;

        void Reset()
        {
            field = new List<Vector2Int>(playableField);
            foreach (var pos in field)
            {
                if (_map[pos.y, pos.x] != Define.AreaTileType.ForceEmpty)
                    _map[pos.y, pos.x] = Define.AreaTileType.Empty;
                _baseTiles[pos.y, pos.x].DisableDecoration();
            }
        }
    }
    private void GenerateObstacles(List<Vector2Int> field, float proportion)
    {
        int totalGenerated = 0;
        int totalPosCount = field.Count;

        while ((float)(totalGenerated) / totalPosCount < proportion)
        {
            Vector2Int pos = field[Random.Range(0, field.Count)];
            if (_map[pos.y, pos.x] == Define.AreaTileType.ForceEmpty) continue;
            _baseTiles[pos.y, pos.x].EnableDecoration();
            _map[pos.y, pos.x] = Define.AreaTileType.Obstacle;
            totalGenerated++;
            field.Remove(pos);
        }
    }

    public void GenerateEventTiles()
    {
        CurrentGeneratePhase = MapGeneratePhase.EventTileGenerate;
        // 시작 지점
        CreateEventTile(_playerStartPosition.x - _playableAreaWidthStartOffset, _playerStartPosition.y - _playableAreaHeightStartOffset, Define.AreaTileType.Start);
        // 보스 타일
        CreateEventTile(_bossPosition.x - _playableAreaWidthStartOffset, _bossPosition.y - _playableAreaHeightStartOffset, Define.AreaTileType.Boss);
        // 전투 타일
        GenerateTileWithWindow(2, _data.BattleTileNum, Define.AreaTileType.Battle);
        // 인카운터 타일
        GenerateTileWithWindow(2, _data.EncounterTileNum, Define.AreaTileType.Encounter);
        // 일반 타일
        for (int z = 0; z < _eventTiles.GetLength(0); z++)
        {
            for (int x = 0; x < _eventTiles.GetLength(1); x++)
            {
                if (_map[z + _playableAreaHeightStartOffset, x + _playableAreaWidthStartOffset] == Define.AreaTileType.Empty)
                {
                    CreateEventTile(x, z, Define.AreaTileType.Normal);
                }
            }
        }
        
    }

    private void GenerateTileWithWindow(int windowSize, int tilenum, Define.AreaTileType tileType)
    {
        int[] windowStartArr;
        ResetWindow();

        int count = 0;
        while (count < tilenum)
        {
            // 모든 windowstart가 한번씩 다 선택됐다면 다시 초기화
            if (windowStartArr.Length == 0)
            {
                ResetWindow();
            }
            int x, z;
            int trycnt = 0;
            while (true)
            {
                trycnt++;
                // z 좌표: 랜덤으로 선택된 windowstart를 시작으로 windowsize만큼의 범위에서 랜덤 선택
                int windowstart = windowStartArr[Random.Range(0, windowStartArr.Length)];
                z = Random.Range(windowstart, windowstart + windowSize - 1);
                // x 좌표는 그냥 width 범위에서 랜덤
                x = Random.Range(0, _data.PlayableMapWidth);
                // 빈 타일이어야 하며, 근처 1칸 범위에 같은 종류 타일이 없어야 하며, 경로가 있어야 함
                if (_map[z + _playableAreaHeightStartOffset, x + _playableAreaWidthStartOffset] == Define.AreaTileType.Empty 
                    && !HasNeighborOfType(x + _playableAreaWidthStartOffset, z + _playableAreaHeightStartOffset, tileType)
                    && FindPath(new Vector2Int(x + _playableAreaWidthStartOffset, z + _playableAreaHeightStartOffset), out var path)) break;
                if (trycnt == 100)
                {
                    // 해당 z좌표에 더 생성할 수 없음 -> all random 시도
                    GenerateTileWithAllRandom(tileType);
                    break;
                }
            }

            CreateEventTile(x, z, tileType);
            count++;

            // 한 번 선택된 windowstart는 다시 선택되지 않음 -> 한 곳에 타일이 몰리는 것을 방지
            windowStartArr = windowStartArr.Where(n => n != z).ToArray();
        }
        return;

        void ResetWindow()
        {
            windowStartArr = new int[_data.PlayableMapHeight - windowSize - 1];
            for (int i = 2; i < _data.PlayableMapHeight - windowSize; i++)
            {
                windowStartArr[i - 2] = i;
            }
        }
    }

    private void GenerateTileWithAllRandom(Define.AreaTileType tileType)
    {
        int x, z;
 
        int trycnt = 0;
        while (true)
        {
            trycnt++;
            // z 좌표: 시작 지점 + 1칸, 보스 타일 제외한 height 범위에서 랜덤
            z = Random.Range(2, _data.PlayableMapHeight - 2);
            // x 좌표: width 범위에서 랜덤
            x = Random.Range(0, _data.PlayableMapWidth);
            if (_map[z + _playableAreaHeightStartOffset, x + _playableAreaWidthStartOffset] == Define.AreaTileType.Empty
                && !HasNeighborOfType(x + _playableAreaWidthStartOffset, z + _playableAreaHeightStartOffset, tileType)
                && FindPath(new Vector2Int(x + _playableAreaWidthStartOffset, z + _playableAreaHeightStartOffset), out var path)) break;
            if (trycnt == 100)
            {
                Debug.LogError($"Could not choose {tileType} position!");
                return;
            }
        }
        CreateEventTile(x, z, tileType);
    }
}
