using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 맵 생성에 필요한 각종 헬퍼 메소드 및 디버그용 메소드 보유
public partial class AreaMapGenerator
{
    private int _playableFieldZStart => Map.PlayableFieldStart.y; // 플레이 영역이 시작되는 z 좌표
    private int _playableFieldXStart => Map.PlayableFieldStart.x; // 플레이 영역이 시작되는 x 좌표
    private Vector2Int _playerStartPosition => Map.PlayerStartPosition; // 플레이어 시작 지점의 Grid 좌표
    private Vector2Int _bossPosition => Map.BossPosition; // 보스 타일 지점의 Grid 좌표

    private string[] _lightCullingMask = new[] { "Player", "AreaLightTarget" };

    public enum MapGeneratePhase
    {
        NotStarted,
        SubtileGenerate,
        Maintilegenerate,
        PlayableFieldSetup,
        UnplayableFieldObstacleGenerate,
        PlayableFieldObstacleGenerate,
        EventTileGenerate,
        FogOfWarGenerate,
    }


    // 시작 위치부터 특정 위치까지 길찾기 (다익스트라)
    // 타일 종류별로 거리에 가중치를 부여하거나 맵 생성 시 보스 타일까지의 거리의 범위를 정하는 등 확장 가능해보임.
    private bool FindPath(Vector2Int destination, out List<Vector2Int> path)
    {
        // 타일별 최단거리
        Dictionary<Vector2Int, int> distances = new()
        {
            [_playerStartPosition] = 0
        };
        Dictionary<Vector2Int, Vector2Int> previous = new(); // 타일의 이전 타일 저장 (경로 추적용)
        path = new List<Vector2Int>(); // 최단 경로 저장할 리스트
        List<Vector2Int> queue = new() { _playerStartPosition };

        while (queue.Count > 0)
        {
            Vector2Int currentNode = GetClosestNode();

            if (currentNode == destination)
            {
                // 목적지 타일 도달한 경우 최단 경로 반환
                while (currentNode != _playerStartPosition)
                {
                    path.Add(currentNode);
                    currentNode = previous[currentNode];
                }
                path.Add(currentNode);
                return true;
            }

            foreach (Vector2Int neighbor in Map.GetNeighbors(currentNode))
            {
                if (Map.TileTypeMap[neighbor.y, neighbor.x] == AreaTileType.OutOfField ||
                    Map.TileTypeMap[neighbor.y, neighbor.x] == AreaTileType.Obstacle) continue;

                int distanceToNeighbor = distances[currentNode] + 1;
                if (!distances.ContainsKey(neighbor) || distanceToNeighbor < distances[neighbor])
                {
                    distances[neighbor] = distanceToNeighbor;
                    previous[neighbor] = currentNode;
                    queue.Add(neighbor);
                }
            }
        }

        // 목적지 타일로 가는 경로가 없음
        return false;

        Vector2Int GetClosestNode()
        {
            // 큐에서 가장 짧은 거리의 노드를 선택
            Vector2Int closestNode = queue[0];
            foreach (Vector2Int node in queue)
            {
                if (distances[node] < distances[closestNode])
                {
                    closestNode = node;
                }
            }
            queue.Remove(closestNode);
            return closestNode;
        }
    }

    /// <summary>
    /// AreaBaseTile Init. AreaBaseTile의 Start에서 할 시 제대로 적용이 안 됨.
    /// </summary>
    private void InitBaseTiles()
    {
        for (int z = 0; z < Map.BaseTileMap.GetLength(0); z++)
        {
            for (int x = 0; x < Map.BaseTileMap.GetLength(1); x++)
            {
                Map.BaseTileMap[z, x].Init();
            }
        }
    }

    /// <summary>
    /// 전체 맵 중 비어있는(AreaTileType.Empty) 위치들 반환
    /// </summary>
    private List<Vector2Int> GetEmptyPositions()
    {
        List<Vector2Int> emptyPositions = new();
        for (int z = 0; z < _data.MapHeight; z++)
        {
            for (int x = 0; x < _data.MapWidth; x++)
            {
                if (Map.TileTypeMap[z, x] == AreaTileType.Empty)
                {
                    emptyPositions.Add(new Vector2Int(x, z));
                }
            }
        }

        return emptyPositions;
    }

    #region Debug
    private void ClearMap()
    {
        GameObject[] parents = new GameObject[5] { GameObject.Find("@Debug"), GameObject.Find("@SubTiles"), GameObject.Find("@MainTiles"), GameObject.Find("@EventTiles"), GameObject.Find("@FogOfWar") };

        foreach (var p in parents)
        {
            if (p == null) continue;
            var pt = p.transform;
            for (int i = pt.childCount - 1; i >= 0; i--)
            {
                Destroy(pt.GetChild(i).gameObject);
            }
        }
    }

    public static void ClearDebugObjects()
    {
        if (GameObject.Find("@Debug"))
        {
            Transform parent = GameObject.Find("@Debug").transform;
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }
    }

    public void ShowTileTypeText()
    {
        if (CurrentGeneratePhase < MapGeneratePhase.EventTileGenerate)
        {
            Debug.LogWarning("[AreaMapGenerator] Map must be generated first!");
            return;
        }

        ClearDebugObjects();

        for (int z = 0; z < Map.TileTypeMap.GetLength(0); z++)
        {
            for (int x = 0; x < Map.TileTypeMap.GetLength(1); x++)
            {
                GameObject canvas = Instantiate(_infoText, Map.GridToWorldPosition(x, z, 2), Quaternion.Euler(60, 0, 0), _debugObjectParent);

                TextMeshProUGUI text = canvas.GetComponentInChildren<TextMeshProUGUI>();
                text.SetText(Map.TileTypeMap[z, x].ToString());
                switch (Map.TileTypeMap[z, x])
                {
                    case AreaTileType.Battle:
                        text.color = Color.red;
                        break;
                    case AreaTileType.Encounter:
                        text.color = Color.yellow;
                        break;
                    case AreaTileType.OutOfField:
                        text.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                        break;
                    case AreaTileType.Boss:
                        text.color = new Color(0.5f, 0.1f, 0.5f);
                        break;
                    case AreaTileType.Start:
                        text.color = Color.cyan;
                        break;
                    case AreaTileType.Normal:
                        text.color = new Color(1, 1, 1, 0.5f);
                        break;
                }
            }
        }
    }

    public void ShowGridPositionText()
    {
        if (CurrentGeneratePhase == MapGeneratePhase.NotStarted)
        {
            Debug.LogWarning("[AreaMapGenerator] Map must be generated first!");
            return;
        }

        ClearDebugObjects();

        for (int z = 0; z < Map.TileTypeMap.GetLength(0); z++)
        {
            for (int x = 0; x < Map.TileTypeMap.GetLength(1); x++)
            {
                GameObject canvas = Instantiate(_infoText, Map.GridToWorldPosition(x, z, 2), Quaternion.Euler(60, 0, 0), _debugObjectParent);
                canvas.GetComponentInChildren<TextMeshProUGUI>().SetText($"{x}, {z}");
            }
        }
    }

    public void ShowPathToBoss()
    {
        if (CurrentGeneratePhase < MapGeneratePhase.PlayableFieldObstacleGenerate)
        {
            Debug.LogWarning("[AreaMapGenerator] Playable Field Decoration must be generated first!");
            return;
        }

        ClearDebugObjects();
        FindPath(_bossPosition, out var path);

        foreach (var pos in path)
        {
            Vector3 position = Map.GridToWorldPosition(pos.x, pos.y, 1.07f);
            Instantiate(_pathIndicator, position, Quaternion.Euler(90, 0, 0), _debugObjectParent);
        }
    }

    public void ShowFogOfWar()
    {
        for (int z = 0; z < _data.MapHeight; z++)
        {
            for (int x = 0; x < _data.MapWidth; x++)
            {
                if (Map.FogOfWarMap[z, x] != null)
                {
                    Map.FogOfWarMap[z, x].Show();
                    Map.BaseTileMap[z, x].DisableObstacle();
                }
            }
        }
    }

    public void HideFogOfWar()
    {
        for (int z = 0; z < _data.MapHeight; z++)
        {
            for (int x = 0; x < _data.MapWidth; x++)
            {
                if (Map.FogOfWarMap[z, x] != null)
                {
                    Map.FogOfWarMap[z, x].Hide();
                }
                Map.BaseTileMap[z, x].EnableObstacle();
            }
        }
    }

    private void Start()
    {
        if (Managers.SceneMng.LastSceneType == SceneType.UnknownScene)
        {
            _isTestMode = true;
        }
    }

    #endregion
}
