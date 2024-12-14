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
    // 맵의 원점
    [SerializeField] private Vector3 _mapOriginPosition;

    // 인스펙터를 통해 커스터마이징 가능한 모든 구역의 맵 생성용 데이터셋
    [SerializeField] private SerializedDictionary<Define.AreaName, AreaMapData> _dataset;
    // 데이터셋 중 현재 생성할 area의 데이터
    private AreaMapData _data;

    private Define.AreaTileType[,] _tileTypeMap;
    private AreaBaseTile[,] _baseTileMap;
    private AreaEventTile[,] _eventTileMap; // Note: 위 두 배열과 다르게 PlayableMapHeight/Width 크기임

    private Transform _subtileParent;
    private Transform _maintileParent;
    private Transform _debugObjectParent;
    private Light _light;

    [Header("Debug")]
    [SerializeField, Tooltip("테스트 용으로 생성할 Area")] private Define.AreaName _testAreaName;

    [SerializeField] private GameObject _infoText;
    [SerializeField] private GameObject _pathIndicator;
    [ReadOnly] public MapGeneratePhase CurrentGeneratePhase = MapGeneratePhase.NotStarted; // 에디터상에서 버튼으로 맵 생성을 테스트하는 데 사용

    public void Init(Define.AreaName area = Define.AreaName.Forest)
    {   
        // 에디터상에서 디버그용
        #if UNITY_EDITOR
        area = _testAreaName;
        ClearMap();
        #endif

        _data = _dataset[area];
        _baseTileMap = new AreaBaseTile[_data.MapHeight, _data.MapWidth];
        _eventTileMap = new AreaEventTile[_data.PlayableMapHeight, _data.PlayableMapWidth];
        _tileTypeMap = new Define.AreaTileType[_data.MapHeight, _data.MapWidth];

        for (int i = 0; i < _data.MapHeight; i++)
        {   
            for (int j = 0; j < _data.MapWidth; j++)
            {
                _tileTypeMap[i, j] = Define.AreaTileType.Empty;
            }
        }

        GameObject mapObject = GameObject.Find("@Map") ?? Managers.ResourceMng.Instantiate("Area/@Map");
        _debugObjectParent = GameObject.Find("@Debug").transform;
        _subtileParent = GameObject.Find("@SubTiles").transform;
        _maintileParent = GameObject.Find("@MainTiles").transform;
        _light = GameObject.FindGameObjectWithTag("AreaLight").GetComponent<Light>();

        _playerStartPosition = new(_data.MapWidth / 2, _playableAreaHeightStartOffset);
        _bossPosition = new(_data.MapWidth / 2, _playableAreaHeightStartOffset + _data.PlayableMapHeight - 1);
    }

    public void GenerateMap()
    {
        GenerateSubtiles();
        GenerateMainTile();
        SetupPlayableField(out List<Vector2Int> playableField, out List<Vector2Int> unplayableField);
        GenerateUnplayableFieldObstacles(unplayableField);
        GeneratePlayableFieldObstacles(playableField);
        GenerateEventTiles();
    }

    // Subtile 생성 알고리즘
    public void GenerateSubtiles()
    {
        AreaSubTileGroupData[] subTileGroupDatas = _data.SubTileGroupData;
        if (subTileGroupDatas.Length == 0) return;

        // 전체 맵에서 차지하는 Subtile 비율의 합이 1을 넘으면 생성 불가
        if (subTileGroupDatas.Sum(x => x.Proportion) > 1)
        {
            Debug.LogError("Sum of subtiles' Proportion is over 1!");
            return;
        }

        CurrentGeneratePhase = MapGeneratePhase.SubtileGenerate;

        // 타일이 맵에 최대한 골고루 퍼지도록 하기 위해 사용되는 배열: 각 행/열 별로 생성된 타일 수의 합
        // numOfsubtiles[i,j]: i행에 생성된 subtile 수 + j열에 생성된 subtile수
        int[,] numOfSubtiles = new int[_data.MapHeight, _data.MapWidth];
        

        // Subtile은 여러개일 수 있음
        foreach (AreaSubTileGroupData subTileGroupData in subTileGroupDatas)
        {
            subTileGroupData.Init();
            int totalGenerated = 0;

            // Subtile 개수는 데이터에서 정해진 비율에 완벽히 맞추진 않으나 최대한 비슷하도록 타일 생성
            while ((float)(totalGenerated) / (_data.MapHeight * _data.MapWidth) < subTileGroupData.Proportion)
            {
                // 생성될 Subtile 그룹의 길이
                int length = Random.Range(subTileGroupData.MinLength, subTileGroupData.MaxLength);

                // 타일이 생성될 위치
                Vector2Int tilePosition;
                // 행/열 합하여 가장 적은 수의 타일이 생성된 위치를 가져옴
                Util.FindMinIndex(numOfSubtiles, out int x, out int z);
                
                // 낮은 확률로 위에서 가져온 위치가 이미 비어있지 않을 수 있음. 그럴 시 빈 타일 중 랜덤 선택.
                if (_tileTypeMap[z, x] != Define.AreaTileType.Empty)
                {
                    List<Vector2Int> emptyPositions = GetEmptyPositions();
                    tilePosition = emptyPositions[Random.Range(0, emptyPositions.Count)];
                }
                else
                {
                    tilePosition = new Vector2Int(x, z);
                }

                Transform tileGroupParent = MakeTileGroupParent(x, z);

                for (int i = 0; i < length; i++)
                {
                    x = tilePosition.x;
                    z = tilePosition.y;
                    Vector3 worldPos = GridToWorldPosition(x, z);

                    // 랜덤한 타일 선택해서 가져옴; 타일 별로 배치된 장식물이 다름
                    AreaBaseTileData tileData = subTileGroupData.SelectRandomTile();

                    // 타일 배치
                    AreaBaseTile tile = Instantiate(tileData.Tile, worldPos, Quaternion.identity, tileGroupParent).GetComponent<AreaBaseTile>();
                    _baseTileMap[z, x] = tile;
                    _tileTypeMap[z,x] = Define.AreaTileType.SubTile;
                    totalGenerated++;
                    IncreaseNumOfSubtiles(x, z);

                    Vector2Int[] emptyNeighbors =
                        GetNeighbors(x, z).Where(pos => _tileTypeMap[pos.y, pos.x] == Define.AreaTileType.Empty).ToArray();

                    if (emptyNeighbors.Length == 0)
                    {
                        break;
                    }

                    tilePosition = emptyNeighbors[Random.Range(0, emptyNeighbors.Length)];
                }
                subTileGroupData.OnNextTilegroupStart();
            }
        }
        return;

        // numOfSubtiles 배열을 조작하는 헬퍼 함수
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

        // 타일 그룹의 부모 생성
        Transform MakeTileGroupParent(int x, int z)
        {
            Transform tileGroupParent = new GameObject("TileGroup").transform;
            tileGroupParent.SetParent(_subtileParent);
            tileGroupParent.position = GridToWorldPosition(x, z);

            return tileGroupParent;
        }

    }

    // MainTile 생성 알고리즘
    public void GenerateMainTile()
    {
        CurrentGeneratePhase = MapGeneratePhase.Maintilegenerate;
        AreaTileGroupData mainTileGroupData = _data.MainTileGroupData;
        mainTileGroupData.Init();

        // Subtile 생성 완료 후 맵에서 빈 위치들을 리스트에 저장
        List<Vector2Int> emptyPositions = GetEmptyPositions();

        // 남은 빈 위치에 생성
        foreach (var pos in emptyPositions)
        {
            int x = pos.x;
            int z = pos.y;
            Vector3 worldPos = GridToWorldPosition(x, z);

            AreaBaseTileData tileData = mainTileGroupData.SelectRandomTile();
            AreaBaseTile tile = Instantiate(tileData.Tile, worldPos, Quaternion.identity, _maintileParent).GetComponent<AreaBaseTile>();

            _baseTileMap[z, x] = tile;
            _tileTypeMap[z, x] = Define.AreaTileType.MainTile;
        }
    }

    // 플레이 가능(플레이어가 이동 가능) 영역 설정
    public void SetupPlayableField(out List<Vector2Int> playableField, out List<Vector2Int> unplayableField)
    {
        InitBaseTiles();

        CurrentGeneratePhase = MapGeneratePhase.PlayableFieldSetup;
        _light.cullingMask = LayerMask.GetMask(_lightCullingMask);
  
        var tempPlayablePos = new List<Vector2Int>();
        unplayableField = new List<Vector2Int>();
        
        int xStart = _data.MapWidth / 2;
        int zStartOffset = 0, zEndOffset = 0;

        // 육각형꼴의 플레이 가능 필드 생성
        for (int xOffset = 0; xOffset <= _data.PlayableMapWidth / 2; xOffset++)
        {
            for (int z = _playableAreaHeightStartOffset + zStartOffset;
                 z < _playableAreaHeightStartOffset + _data.PlayableMapHeight - zEndOffset;
                 z++)
            {   
                // 타일을 플레이 가능 필드로 설정. 해당 타일의 AreaTileType은 다시 Empty가 됨.
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

        // 플레이 불가능한 필드의 좌표를 unplayableField 리스트에 추가
        for (int z = 0; z < _data.MapHeight; z++)
        {
            for (int x = 0; x < _data.MapWidth; x++)
            {   
                // 이 시점에서 타일의 tileType이 Empty라면 플레이 가능 필드라는 의미
                if (_tileTypeMap[z, x] == Define.AreaTileType.Empty)
                    continue;
                unplayableField.Add(new Vector2Int(x, z));
                _tileTypeMap[z, x] = Define.AreaTileType.OutOfField;
            }
        }

        // 시작 타일 및 보스 타일과 그 이웃 타일들은 ForceEmpty타입으로 설정하여 장애물을 배치하지 않도록 함
        List<Vector2Int> forceEmptyTilePositions = new() { _playerStartPosition, _bossPosition };
        foreach (var tilePos in new List<Vector2Int>{_playerStartPosition, _bossPosition})
        {
            var neighbors = GetNeighbors(tilePos);
            foreach (var pos in neighbors)
            {
                if (_tileTypeMap[pos.y, pos.x] != Define.AreaTileType.OutOfField)
                {
                    forceEmptyTilePositions.Add(pos);
                }
            }
        }

        foreach (var pos in forceEmptyTilePositions)
        {
            _tileTypeMap[pos.y, pos.x] = Define.AreaTileType.ForceEmpty;
        }

        playableField = tempPlayablePos;
        return;

        void SetPlayableTile(int x, int z)
        {
            _baseTileMap[z, x].EnableLight();
            _tileTypeMap[z, x] = Define.AreaTileType.Empty;
            tempPlayablePos.Add(new Vector2Int(x, z));
        }
    }

    // 플레이 불가능 필드의 장애물(장식물) 생성
    public void GenerateUnplayableFieldObstacles(List<Vector2Int> unplayableField)
    {
        CurrentGeneratePhase = MapGeneratePhase.UnplayableFieldObstacleGenerate;
        GenerateObstacles(unplayableField, _data.UnplayableFieldDecorationProportion);
    }

    // 플레이 가능 필드의 장애물(장식물) 생성
    public void GeneratePlayableFieldObstacles(List<Vector2Int> playableField)
    {
        CurrentGeneratePhase = MapGeneratePhase.PlayableFieldObstacleGenerate;

        List<Vector2Int> field;
        List<Vector2Int> path;
        float proportion = _data.PlayableFieldDecorationProportion;
        int trycount = 0;

        // 보스에게 가는 경로가 존재할 때까지 장애물 생성과 리셋 반복
        // 100회 실패할 때마다 맵에서 장애물 비율을 강제로 0.05씩 감소시킴
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
                if (_tileTypeMap[pos.y, pos.x] != Define.AreaTileType.ForceEmpty)
                    _tileTypeMap[pos.y, pos.x] = Define.AreaTileType.Empty;
                _baseTileMap[pos.y, pos.x].DisableDecoration();
            }
        }
    }

    // 필드에 비율에 맞게 장애물 생성
    private void GenerateObstacles(List<Vector2Int> field, float proportion)
    {
        int totalGenerated = 0;
        int totalPosCount = field.Count;

        while ((float)(totalGenerated) / totalPosCount < proportion)
        {
            Vector2Int pos = field[Random.Range(0, field.Count)];
            if (_tileTypeMap[pos.y, pos.x] == Define.AreaTileType.ForceEmpty) continue;
            _baseTileMap[pos.y, pos.x].EnableDecoration();
            _tileTypeMap[pos.y, pos.x] = Define.AreaTileType.Obstacle;
            totalGenerated++;
            field.Remove(pos);
        }
    }

    // 플레이 가능 영역에 이벤트 타일 생성
    public void GenerateEventTiles()
    {
        CurrentGeneratePhase = MapGeneratePhase.EventTileGenerate;

        // 시작 좌표와 보스 좌표를 플레이 가능 영역에서의 좌표로 변환
        Vector2Int startPos = new(_playerStartPosition.x - _playableAreaWidthStartOffset,
            _playerStartPosition.y - _playableAreaHeightStartOffset);

        Vector2Int bossPos = new(_bossPosition.x - _playableAreaWidthStartOffset,
            _bossPosition.y - _playableAreaHeightStartOffset);
        // 시작 지점
        CreateEventTile(startPos.x, startPos.y, Define.AreaTileType.Start);
        // 보스 타일
        CreateEventTile(bossPos.x, bossPos.y, Define.AreaTileType.Boss);
        // 전투 타일
        CreateTileWithWindow(2, _data.BattleTileNum, Define.AreaTileType.Battle);
        // 인카운터 타일
        CreateTileWithWindow(2, _data.EncounterTileNum, Define.AreaTileType.Encounter);
        // 일반 타일
        for (int z = 0; z < _eventTileMap.GetLength(0); z++)
        {
            for (int x = 0; x < _eventTileMap.GetLength(1); x++)
            {
                var tileType = _tileTypeMap[z + _playableAreaHeightStartOffset, x + _playableAreaWidthStartOffset];
                if (tileType is Define.AreaTileType.Empty or Define.AreaTileType.ForceEmpty)
                {
                    CreateEventTile(x, z, Define.AreaTileType.Normal);
                }
            }
        }
        
    }

    // Window를 사용하여 이벤트 타일 생성. 랜덤성을 높이고자 한 시도
    private void CreateTileWithWindow(int windowSize, int tilenum, Define.AreaTileType tileType)
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
                // x 좌표는 단순히 width 범위에서 랜덤
                x = Random.Range(0, _data.PlayableMapWidth);
                // 빈 타일이어야 하며, 근처 1칸 범위에 같은 종류 타일이 없어야 하며, 경로가 있어야 함
                if (_tileTypeMap[z + _playableAreaHeightStartOffset, x + _playableAreaWidthStartOffset] == Define.AreaTileType.Empty 
                    && !HasNeighborOfType(x + _playableAreaWidthStartOffset, z + _playableAreaHeightStartOffset, tileType)
                    && FindPath(new Vector2Int(x + _playableAreaWidthStartOffset, z + _playableAreaHeightStartOffset), out var path)) break;
                if (trycnt == 100)
                {
                    // 해당 z좌표에 더 생성할 수 없음 -> all random 시도
                    CreateTileWithAllRandom(tileType);
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

    // 완전한 랜덤으로 이벤트 타일 생성
    private void CreateTileWithAllRandom(Define.AreaTileType tileType)
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
            if (_tileTypeMap[z + _playableAreaHeightStartOffset, x + _playableAreaWidthStartOffset] == Define.AreaTileType.Empty
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
