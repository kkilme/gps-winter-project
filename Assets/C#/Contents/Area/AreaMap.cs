using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// XZ를 축으로 하는 육각형 그리드
// 육각형은 평평한 부분이 위 (flat-top)
public class AreaMap
{
    public Define.AreaTileType[,] TileTypeMap { get; }
    public AreaEventTile[,] EventTileMap { get; }
    public AreaBaseTile[,] BaseTileMap { get; }

    public Vector2Int PlayableFieldStart; // 플레이 영역이 시작되는 Grid 좌표 (x, z)
    public Vector2Int PlayerStartPosition; // 플레이어 시작 지점의 Grid 좌표
    public Vector2Int BossPosition; // 보스 타일 지점의 Grid 좌표

    private Vector3 _originPosition; // 맵 원점: Grid 좌표 (0,0)의 월드 좌표
    private int _width; // Grid 단위
    private int _height; // Grid 단위
    private int _playableFieldWidth; // Grid 단위
    private int _playableFieldHeight; // Grid 단위

    private const float TILE_PREFAB_WIDTH = 4;
    private const float TILE_PREFAB_HEIGHT = 3.5f;

    public AreaMap(int width, int height, int playableFieldWidth, int playableFieldHeight, Vector3 originPosition)
    {
        _width = width;
        _height = height;
        _playableFieldWidth = playableFieldWidth;
        _playableFieldHeight = playableFieldHeight;
        _originPosition = originPosition;
        TileTypeMap = new Define.AreaTileType[height, width];
        EventTileMap = new AreaEventTile[height, width];
        BaseTileMap = new AreaBaseTile[height, width];

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                TileTypeMap[z, x] = Define.AreaTileType.Empty;
            }
        }

        PlayableFieldStart = new Vector2Int(_width / 2 - _playableFieldWidth / 2, _height / 2 - _playableFieldHeight / 2);
        PlayerStartPosition = new Vector2Int(_width / 2, PlayableFieldStart.y);
        BossPosition = new Vector2Int(_width / 2, PlayableFieldStart.y + _playableFieldHeight - 1);
    }

    // 그리드 좌표를 월드 좌표로 변환
    public Vector3 GridToWorldPosition(int x, int z, float y = 0)
    {
        if (x % 2 == 1) return new Vector3(x * TILE_PREFAB_WIDTH * 0.75f, y, (z + 0.5f) * TILE_PREFAB_HEIGHT) + _originPosition;
        else return new Vector3(x * TILE_PREFAB_WIDTH * 0.75f, y, z * TILE_PREFAB_HEIGHT) + _originPosition;
    }

    // 월드 좌표를 그리드 좌표로 변환
    public void WorldToGridPosition(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.RoundToInt((worldPosition.x - (int)_originPosition.x) / (TILE_PREFAB_WIDTH * 0.75f));
        z = Mathf.RoundToInt((worldPosition.z - (int)_originPosition.z) / TILE_PREFAB_HEIGHT - (x % 2 == 1 ? 0.5f : 0f));
    }

    // 월드 좌표를 해당하는 그리드 좌표 타일의 중심 월드 좌표로 변환
    public Vector3 GetTileCenterPosition(Vector3 worldPosition)
    {
        WorldToGridPosition(worldPosition, out int x, out int z);
        return GridToWorldPosition(x, z, 1.02f);
    }

    // 플레이어 시작 지점을 월드 좌표로 반환
    public Vector3 GetPlayerStartPosition()
    {
        return GridToWorldPosition(PlayerStartPosition.x, PlayerStartPosition.y, 1.04f);
    }

    // 보스 지점을 월드 좌표로 반환
    public Vector3 GetBossPosition()
    {
        return GridToWorldPosition(BossPosition.x, BossPosition.y, 1.04f);
    }

    public AreaEventTile GetEventTile(Vector3 worldPosition)
    {
        WorldToGridPosition(worldPosition, out int x, out int z);
        return EventTileMap[z, x];
    }

    public Define.AreaTileType GetTileType(Vector3 worldPosition)
    {
        WorldToGridPosition(worldPosition, out int x, out int z);
        return TileTypeMap[z, x];
    }

    private bool IsPositionValid(int x, int z)
    {
        return x >= 0 && x < _width && z >= 0 && z < _height;
    }

    // 플레이어가 서 있을 수 있는 타일인지 확인
    // 이는 단순히 플레이어가 위에 서 있을 수 있는 타일인지 확인하는 것이지, 실제 게임 도중 플레이어가 현재 위치에서 이동 가능한 타일인지 확인하는 게 아님.
    // 게임 도중 이동 가능한지 체크는 IsPositionMoveable에서 진행 
    public bool IsPositionStandable(int x, int z)
    {
        if (!IsPositionValid(x, z)) return false;
        return TileTypeMap[z, x] != Define.AreaTileType.Obstacle && TileTypeMap[z, x] != Define.AreaTileType.OutOfField;
    }
    public bool IsPositionStandable(Vector3 worldPosition)
    {   
        WorldToGridPosition(worldPosition, out int x, out int z);
        return IsPositionStandable(x, z);
    }
    // 게임 도중 현재 플레이어 위치에서 대상 위치가 이동 가능한 위치인지 확인
    public bool IsPositionMoveable(Vector3 currentPlayerPosition, Vector3 targetPosition)
    {
        return IsPositionStandable(targetPosition) && IsNeighbor(currentPlayerPosition, targetPosition);
    }

    // (x,z) 타일의 이웃 타일 반환
    public List<Vector2Int> GetNeighbors(int x, int z)
    {
        int[,] dir = x % 2 == 0
            ? new[,] { { 0, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 } }
            : new[,] { { 0, 1 }, { 1, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 }, { -1, 1 } };

        List<Vector2Int> neighbors = new();

        for (int i = 0; i < 6; i++)
        {
            int newx = x + dir[i, 0];
            int newz = z + dir[i, 1];
            if (IsPositionValid(newx, newz))
            {
                neighbors.Add(new Vector2Int(newx, newz));
            }
        }

        return neighbors;
    }

    public List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        return GetNeighbors(pos.x, pos.y);
    }

    // 두 좌표에 해당하는 타일이 이웃인지 확인
    private bool IsNeighbor(int originx, int originz, int targetx, int targetz)
    {
        List<Vector2Int> neighbors = GetNeighbors(originx, originz);
        foreach (var neighbor in neighbors)
        {
            if (neighbor.x == targetx && neighbor.y == targetz) return true;
        }
        return false;
    }

    public bool IsNeighbor(Vector3 origin, Vector3 target)
    {
        WorldToGridPosition(origin, out int x1, out int z1);
        WorldToGridPosition(target, out int x2, out int z2);
        return IsNeighbor(x1, z1, x2, z2);
    }

    // 해당 셀의 이웃 중 tileType인 타일이 하나라도 있다면 true, 하나도 없다면 false 반환
    public bool HasNeighborOfType(int x, int z, Define.AreaTileType tileType)
    {
        List<Vector2Int> neighbors = GetNeighbors(x, z);
        foreach (var neighbor in neighbors)
        {
            if (TileTypeMap[neighbor.y, neighbor.x] == tileType) return true;
        }

        return false;
    }

    // 해당 타일의 이웃 타일들 색 변경
    public void ChangeNeighborTilesColor(Vector3 worldPosition, TileColorChangeType colorChangeType)
    {
        WorldToGridPosition(worldPosition, out int x, out int z);
        List<Vector2Int> neighbors = GetNeighbors(x, z);

        foreach (Vector2Int neighbor in neighbors)
        {
            if (IsPositionStandable(neighbor.x, neighbor.y))
            {
                EventTileMap[neighbor.y, neighbor.x].ChangeColor(colorChangeType);
            }
        }
    }

    // 맵 첫 생성 시 뿐 아니라 서든데스 등 게임 진행 도중 타일 타입 바꾸는데에도 필요
    public void CreateEventTile(int x, int z, Define.AreaTileType tileType, bool isReplace = false)
    {
        Vector3 worldPosition = GridToWorldPosition(x, z, 1.02f);

        AreaEventTile tile;

        if (isReplace)
        {   
            var oldTile = EventTileMap[z, x];
            oldTile.Destroy();
            tile = TileFactory.CreateTile(worldPosition, tileType, oldTile.TileObject);
        }
        else
        {
            tile = TileFactory.CreateTile(worldPosition, tileType);
        }

        EventTileMap[z, x] = tile;
        TileTypeMap[z, x] = tileType;
    }

    public void CreateEventTile(Vector3 worldPosition, Define.AreaTileType tileType, bool isReplace = false)
    {
        WorldToGridPosition(worldPosition, out int x, out int z);
        CreateEventTile(x, z, tileType, isReplace);
    }

    public void OnSuddendeath(int z)
    {
        for (int x = PlayableFieldStart.x; x < PlayableFieldStart.x + _playableFieldWidth; x++)
        {
            if (IsPositionStandable(x, z)) CreateEventTile(GridToWorldPosition(x, z), Define.AreaTileType.Destroyed, true);
            // TODO: 플레이어가 서든데스로 파괴된 타일에 있을 시 효과 발동
        }
    }

    public void CalcCameraPosLimitX(out float xmin, out float xmax)
    {
        xmin = GridToWorldPosition(PlayableFieldStart.x, 0).x;
        xmax = GridToWorldPosition(PlayableFieldStart.x + _playableFieldWidth - 1, 0).x;
    }
}
