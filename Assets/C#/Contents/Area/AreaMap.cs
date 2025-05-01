using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// XZ를 축으로 하는 육각형 맵
// 육각형은 평평한 부분이 위 (flat-top)
public class AreaMap
{
    // 맵은 2차원 배열로 표현되며, 좌측 최하단이 (0, 0).
    // 모두  _height x _width 크기임
    // Map[z, x] = (x, z) 좌표의 타일 (x는 가로, z는 세로)
    public AreaTileType[,] TileTypeMap { get; }
    public AreaEventTile[,] EventTileMap { get; }
    public AreaBaseTile[,] BaseTileMap { get; }
    public FogOfWar[,] FogOfWarMap { get; }

    public Vector2Int PlayableFieldStart; // 플레이 영역 좌측 최하단의 Grid 좌표 (x, z)
    public Vector2Int PlayerStartPosition; // 플레이어 시작 지점의 Grid 좌표
    public Vector2Int BossPosition; // 보스 타일 지점의 Grid 좌표

    public int Width { get; private set; }// Grid 단위, 전체 맵 너비
    public int Height { get; private set; }// Grid 단위, 전체 맵 높이
    public int PlayableFieldWidth { get; private set; } // Grid 단위, 플레이어 이동 가능 영역의 너비
    public int PlayableFieldHeight { get; private set; }// Grid 단위, 플레이어 이동 가능 영역의 높이

    private Vector3 _originPosition; // 맵 원점: Grid 좌표 (0,0)의 월드 좌표

    private const float TILE_PREFAB_WIDTH = 4;
    private const float TILE_PREFAB_HEIGHT = 3.5f;

    private Transform _eventTileParent;

    public AreaMap(int width, int height, int playableFieldWidth, int playableFieldHeight, Vector3 originPosition)
    {
        Width = width;
        Height = height;
        PlayableFieldWidth = playableFieldWidth;
        PlayableFieldHeight = playableFieldHeight;
        _originPosition = originPosition;

        TileTypeMap = new AreaTileType[height, width];
        EventTileMap = new AreaEventTile[height, width];
        BaseTileMap = new AreaBaseTile[height, width];
        FogOfWarMap = new FogOfWar[height, width];

        _eventTileParent = GameObject.Find("@EventTiles").transform;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                TileTypeMap[z, x] = AreaTileType.Empty;
            }
        }

        PlayableFieldStart = new Vector2Int(Width / 2 - PlayableFieldWidth / 2, Height / 2 - PlayableFieldHeight / 2);
        PlayerStartPosition = new Vector2Int(Width / 2, PlayableFieldStart.y);
        BossPosition = new Vector2Int(Width / 2, PlayableFieldStart.y + PlayableFieldHeight - 1);
    }

    /// <summary>
    /// 그리드 좌표를 월드 좌표로 변환
    /// </summary>
    public Vector3 GridToWorldPosition(int x, int z, float y = 0)
    {
        if (x % 2 == 1) return new Vector3(x * TILE_PREFAB_WIDTH * 0.75f, y, (z + 0.5f) * TILE_PREFAB_HEIGHT) + _originPosition;
        else return new Vector3(x * TILE_PREFAB_WIDTH * 0.75f, y, z * TILE_PREFAB_HEIGHT) + _originPosition;
    }

    public Vector3 GridToWorldPosition(Vector2Int pos, float y = 0)
    {
        return GridToWorldPosition(pos.x, pos.y, y);
    }

    /// <summary>
    /// 월드 좌표를 그리드 좌표로 변환
    /// </summary>
    public void WorldToGridPosition(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.RoundToInt((worldPosition.x - (int)_originPosition.x) / (TILE_PREFAB_WIDTH * 0.75f));
        z = Mathf.RoundToInt((worldPosition.z - (int)_originPosition.z) / TILE_PREFAB_HEIGHT - (x % 2 == 1 ? 0.5f : 0f));
    }

    /// <summary>
    /// 월드 좌표를 해당하는 그리드 좌표 타일의 중심 월드 좌표로 변환
    /// </summary>
    public Vector3 GetTileCenterPosition(Vector3 worldPosition)
    {
        WorldToGridPosition(worldPosition, out int x, out int z);
        return GridToWorldPosition(x, z, 1.02f);
    }

    /// <summary>
    /// 플레이어 시작 지점을 월드 좌표로 반환
    /// </summary>
    public Vector3 GetPlayerStartWorldPosition()
    {
        return GridToWorldPosition(PlayerStartPosition.x, PlayerStartPosition.y, 1.04f);
    }

    /// <summary>
    /// 보스 지점을 월드 좌표로 반환
    /// </summary>
    public Vector3 GetBossWorldPosition()
    {
        return GridToWorldPosition(BossPosition.x, BossPosition.y, 1.04f);
    }

    /// <summary>
    /// 월드 좌표 위치의 이벤트 타일 반환
    /// </summary>
    public AreaEventTile GetEventTile(Vector3 worldPosition)
    {
        WorldToGridPosition(worldPosition, out int x, out int z);
        return EventTileMap[z, x];
    }

    /// <summary>
    /// 월드 좌표 위치의 타일 타입 반환
    /// </summary>
    public AreaTileType GetTileType(Vector3 worldPosition)
    {
        WorldToGridPosition(worldPosition, out int x, out int z);
        return TileTypeMap[z, x];
    }

    private bool IsPositionValid(int x, int z)
    {
        return x >= 0 && x < Width && z >= 0 && z < Height;
    }

    /// <summary>
    /// 해당 그리드 좌표가 영웅 파티가 서 있을 수 있는 위치인지 확인
    /// </summary>
    public bool IsPositionStandable(int x, int z)
    {
        if (!IsPositionValid(x, z)) return false;
        return TileTypeMap[z, x] != AreaTileType.Obstacle && TileTypeMap[z, x] != AreaTileType.OutOfField;
    }

    public bool IsPositionStandable(Vector3 worldPosition)
    {
        WorldToGridPosition(worldPosition, out int x, out int z);
        return IsPositionStandable(x, z);
    }

    /// <summary>
    /// 게임 진행중 영웅 파티가 대상 위치로 이동 가능한지 확인
    /// </summary>
    public bool IsPositionMoveable(Vector3 currentPlayerPosition, Vector3 targetPosition)
    {
        return IsPositionStandable(targetPosition) && IsNeighbor(currentPlayerPosition, targetPosition);
    }

    /// <summary>
    /// (x,z)의 이웃에 해당하는 좌표 반환
    /// </summary>
    public List<Vector2Int> GetNeighbors(int x, int z)
    {
        int[,] dir = x % 2 == 0
            ? GlobalValues.DIRECTION_6WAY_X_EVEN
            : GlobalValues.DIRECTION_6WAY_X_ODD;

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

    /// <summary>
    /// 두 좌표에 해당하는 타일이 이웃인지 확인
    /// </summary>
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

    /// <summary>
    /// 해당 위치 타일의 이웃 중 tileType인 타일이 하나라도 있다면 true, 하나도 없다면 false 반환
    /// </summary>
    public bool HasNeighborOfType(int x, int z, AreaTileType tileType)
    {
        List<Vector2Int> neighbors = GetNeighbors(x, z);
        foreach (var neighbor in neighbors)
        {
            if (TileTypeMap[neighbor.y, neighbor.x] == tileType) return true;
        }

        return false;
    }

    /// <summary>
    /// 해당 위치 타일의 이웃 타일들 색 변경
    /// </summary>
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

    /// <summary>
    /// 주어진 위치에 이벤트타일 생성.
    /// </summary>
    public void CreateEventTile(int x, int z, AreaTileType tileType)
    {
        Vector3 worldPosition = GridToWorldPosition(x, z, 1.02f);

        AreaEventTile tile = AreaTileFactory.CreateTile(worldPosition, tileType, _eventTileParent);

        EventTileMap[z, x] = tile;
        TileTypeMap[z, x] = tileType;
    }

    public void CreateEventTile(Vector3 worldPosition, AreaTileType tileType)
    {
        WorldToGridPosition(worldPosition, out int x, out int z);
        CreateEventTile(x, z, tileType);
    }

    /// <summary>
    /// 주어진 위치의 이벤트 타일을 파괴하고 tileType 타일로 교체.
    /// </summary>
    public void ReplaceEventTile(int x, int z, AreaTileType tileType)
    {
        var oldTile = EventTileMap[z, x];
        if(oldTile == null)
        {
            CreateEventTile(x, z, tileType);
        } else
        {
            oldTile.Destroy();
            CreateEventTile(x, z, tileType);
        }
    }

    public void ReplaceEventTile(Vector3 worldPosition, AreaTileType tileType)
    {
        WorldToGridPosition(worldPosition, out int x, out int z);
        ReplaceEventTile(x, z, tileType);
    }

    /// <summary>
    /// row(PlayableField 기준)행부터 row + amount - 1 행까지의 타일들을 CollapsedTile로 교체. Boss 타일은 제외.
    /// </summary>
    public void CollapseTiles(int row, int amount)
    {
        for(int z = row + PlayableFieldStart.y; z < row + PlayableFieldStart.y + amount; z++)
        {
            for (int x = PlayableFieldStart.x; x <= PlayableFieldStart.x + PlayableFieldWidth; x++)
            {
                if (TileTypeMap[z, x] != AreaTileType.OutOfField && TileTypeMap[z, x] != AreaTileType.Boss) ReplaceEventTile(GridToWorldPosition(x, z), AreaTileType.Collapsed);
                BaseTileMap[z, x].OnCollapse();
                RevealFogOfWar(x, z); // 전장의 안개도 함께 제거
            }
        }
    }

    /// <summary>
    /// 특정 위치를 기준으로 볼 수 있는 타일들의 위치를 계산하여 반환. 기본 시야 거리: 2
    /// </summary>
    public HashSet<Vector2Int> GetVisibleTilePositions(Vector3 currentPosition, int visionRange = 2)
    {
        WorldToGridPosition(currentPosition, out int currentX, out int currentZ);
        HashSet<Vector2Int> visibleTiles = new();

        void Explore(int x, int z, int currentDistance)
        {
            var pos = new Vector2Int(x, z);
            if (visibleTiles.Contains(pos)) return;
            visibleTiles.Add(pos);

            if (currentDistance >= visionRange) return;

            foreach (var neighbor in GetNeighbors(x, z))
            {
                if (BaseTileMap[z, x].IsObstacleGenerated) // TileTypeMap == Obstacle을 사용하지 않는 이유: UnplayableField에서 모든 타일의 타입은 OutOfField로 지정되기 때문에 Obstacle Type이 존재하지 않음.
                    Explore(neighbor.x, neighbor.y, currentDistance + 2); // 장애물: 거리 2
                else
                    Explore(neighbor.x, neighbor.y, currentDistance + 1); // 일반 타일: 거리 1
            }
        }

        Explore(currentX, currentZ, 0);

        return visibleTiles;
    }

    /// <summary>
    /// 특정 위치와 시야 범위를 사용하여 전장의 안개 제거. 기본 시야 거리: 2
    /// </summary>
    public void RevealFogOfWar(Vector3 currentPosition, int visionRange = 2)
    {
        var fogOfWarsToReveal = GetVisibleTilePositions(currentPosition, visionRange);

        foreach (var pos in fogOfWarsToReveal)
        {
            RevealFogOfWar(pos.x, pos.y);
        }
    }

    /// <summary>
    /// 특정 위치의 전장의 안개 제거
    /// </summary>
    public void RevealFogOfWar(int x, int z)
    {
        // 안개 제거
        if (FogOfWarMap[z, x] != null)
        {
            FogOfWarMap[z, x].Destroy();
            FogOfWarMap[z, x] = null;
        }

        // 장애물 존재하는 타일일 시 장애물 활성화
        if (BaseTileMap[z, x].IsObstacleGenerated) BaseTileMap[z, x].EnableObstacle();
    }

    /// <summary>
    /// Area 시작 시 특정 전장의 안개 제거
    /// </summary>
    public void RevealFogOfWarOnAreaStart()
    {
        RevealFogOfWar(GridToWorldPosition(PlayerStartPosition), 2); // 시작 지점에서 범위 2 반경의 전장의 안개 제거
        RevealFogOfWar(GridToWorldPosition(BossPosition), 1); // 보스 지점에서 범위 1 반경의 전장의 안개 제거
    }

    public void OnAreaStart()
    {
        for (int z = 0; z < Height; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                BaseTileMap[z, x].SetBrightness(false); // 모든 타일의 밝기 낮춤
            }
        }
        RevealFogOfWarOnAreaStart(); // Area 시작 시 전장의 안개 제거
    }

    /// <summary>
    /// AreaCamera의 X좌표 제한 계산 및 반환
    /// </summary>
    public void CalcCameraPosLimitX(out float xmin, out float xmax)
    {
        xmin = GridToWorldPosition(PlayableFieldStart.x, 0).x;
        xmax = GridToWorldPosition(PlayableFieldStart.x + PlayableFieldWidth - 1, 0).x;
    }
}
