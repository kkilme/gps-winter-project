using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 맵 생성에 필요한 각종 헬퍼 메소드 및 디버그용 메소드 보유
public partial class AreaMapGenerator
{
    private int _playableFieldZStart => _map.PlayableFieldStart.y; // 플레이 영역이 시작되는 z 좌표
    private int _playableFieldXStart => _map.PlayableFieldStart.x; // 플레이 영역이 시작되는 x 좌표
    private Vector2Int _playerStartPosition => _map.PlayerStartPosition; // 플레이어 시작 지점의 Grid 좌표
    private Vector2Int _bossPosition => _map.BossPosition; // 보스 타일 지점의 Grid 좌표

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

            foreach (Vector2Int neighbor in _map.GetNeighbors(currentNode))
            {
                if (_map.TileTypeMap[neighbor.y, neighbor.x] == Define.AreaTileType.OutOfField ||
                    _map.TileTypeMap[neighbor.y, neighbor.x] == Define.AreaTileType.Obstacle) continue;

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
            // legacy: 가장 짧은 거리의 노드가 여러 개인 경우 랜덤으로 선택: 굳이 필요해보이진 않음
            //List<Vector2Int> closestNodes = new() {closestNode};
            //foreach (Vector2Int node in queue)
            //{
            //    if (distances[closestNode] == distances[node])
            //    {
            //        closestNodes.Add(node);
            //    }
            //}
            //Vector2Int selected = closestNodes[Random.Range(0, closestNodes.Count)];
            queue.Remove(closestNode);
            return closestNode;
        }
    }

    // AreaBaseTile Init. AreaBaseTile의 Start 메소드에서 할 시 제대로 적용이 안 됨.
    private void InitBaseTiles()
    {
        for (int z = 0; z < _map.BaseTileMap.GetLength(0); z++)
        {
            for (int x = 0; x < _map.BaseTileMap.GetLength(1); x++)
            {
                _map.BaseTileMap[z, x].Init();
            }
        }
    }

    // 전체 맵 중 빈 위치 반환
    private List<Vector2Int> GetEmptyPositions()
    {
        List<Vector2Int> emptyPositions = new();
        for (int z = 0; z < _data.MapHeight; z++)
        {
            for (int x = 0; x < _data.MapWidth; x++)
            {
                if (_map.TileTypeMap[z, x] == Define.AreaTileType.Empty)
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
        GameObject[] parents = new GameObject[4] { GameObject.Find("@Debug"), GameObject.Find("@SubTiles"), GameObject.Find("@MainTiles"), GameObject.Find("@EventTiles") };

        foreach (var p in parents)
        {
            if (p == null) continue;
            var pt = p.transform;
            for (int i = pt.childCount - 1; i >= 0; i--)
            {
                Destroy(pt.GetChild(i).gameObject);
            }
        }

        Debug.Log("Map Cleared");
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
        if (CurrentGeneratePhase != MapGeneratePhase.EventTileGenerate)
        {
            Debug.LogWarning("Map must be generated first!");
            return;
        }

        ClearDebugObjects();

        for (int z = 0; z < _map.TileTypeMap.GetLength(0); z++)
        {
            for (int x = 0; x < _map.TileTypeMap.GetLength(1); x++)
            {
                GameObject canvas = Instantiate(_infoText, _map.GridToWorldPosition(x, z, 2), Quaternion.Euler(60, 0, 0), _debugObjectParent);

                TextMeshProUGUI text = canvas.GetComponentInChildren<TextMeshProUGUI>();
                text.SetText(_map.TileTypeMap[z, x].ToString());
                switch (_map.TileTypeMap[z, x])
                {
                    case Define.AreaTileType.Battle:
                        text.color = Color.red;
                        break;
                    case Define.AreaTileType.Encounter:
                        text.color = Color.yellow;
                        break;
                    case Define.AreaTileType.OutOfField:
                        text.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                        break;
                    case Define.AreaTileType.Boss:
                        text.color = new Color(0.5f, 0.1f, 0.5f);
                        break;
                    case Define.AreaTileType.Start:
                        text.color = Color.cyan;
                        break;
                    case Define.AreaTileType.Normal:
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
            Debug.LogWarning("Map must be generated first!");
            return;
        }

        ClearDebugObjects();

        for (int z = 0; z < _map.TileTypeMap.GetLength(0); z++)
        {
            for (int x = 0; x < _map.TileTypeMap.GetLength(1); x++)
            {
                GameObject canvas = Instantiate(_infoText, _map.GridToWorldPosition(x, z, 2), Quaternion.Euler(60, 0, 0), _debugObjectParent);
                canvas.GetComponentInChildren<TextMeshProUGUI>().SetText($"{z}, {x}");
            }
        }
    }

    public void ShowPathToBoss()
    {
        if (CurrentGeneratePhase != MapGeneratePhase.PlayableFieldObstacleGenerate &&
            CurrentGeneratePhase != MapGeneratePhase.EventTileGenerate)
        {
            Debug.LogWarning("Playable Field Decoration must be generated first!");
            return;
        }

        ClearDebugObjects();
        FindPath(_bossPosition, out var path);

        foreach (var pos in path)
        {
            Vector3 position = _map.GridToWorldPosition(pos.x, pos.y, 1.07f);
            Instantiate(_pathIndicator, position, Quaternion.Euler(90, 0, 0), _debugObjectParent);
        }
    }

    public void ShowFogOfWar()
    {
        for (int z = 0; z < _data.MapHeight; z++)
        {
            for (int x = 0; x < _data.MapWidth; x++)
            {
                if (_map.FogOfWarMap[z, x] != null)
                {
                    _map.FogOfWarMap[z, x].Show();
                    _map.BaseTileMap[z, x].DisableDecoration();
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
                if (_map.FogOfWarMap[z, x] != null)
                {
                    _map.FogOfWarMap[z, x].Hide();
                }
                _map.BaseTileMap[z, x].EnableDecoration();
            }
        }
    }

    public void TestGenerateMap()
    {
        if (!Init()) return;
        GenerateMap();
    }

    private void Start()
    {
        if (Managers.SceneMng.CurrentScene is AreaScene)
        {
            _isTestMode = true;

        }
    }

    #endregion
}
