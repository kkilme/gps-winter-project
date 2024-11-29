using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class AreaMapGenerator
{
    private const float TILE_WIDTH = 4;
    private const float TILE_HEIGHT = 3.5f;
    private const int MAINTILE_GROUP_GENERATE_END_OFFSET = 10;
    private int _playableAreaHeightStartOffset;
    private int _playableAreaWidthStartOffset;
    private Vector2Int _nextTilePosition;
    private Vector2Int _playerStartPosition;
    private Vector2Int _bossPosition;
    private string[] _lightCullingMask = new[] { "Player", "AreaLightTarget" };

    public enum MapGeneratePhase
    {   
        NotStarted,
        SubtileGenerate,
        Maintilegenerate,
        PlayableFieldSetup,
        UnplayableFieldDecorationGenerate,
        PlayableFieldDecorationGenerate,
        EventTileGenerate,
    }

    private void CreateEventTile(int x, int z, Define.AreaTileType tileType)
    {
        Vector3 worldPosition = GridToWorldPosition(x + _playableAreaWidthStartOffset, z + _playableAreaHeightStartOffset, 1.02f);

        AreaEventTile tile = TileFactory.CreateTile(worldPosition, tileType);
  
        _eventTiles[z, x] = tile;
        _map[z + _playableAreaHeightStartOffset, x +_playableAreaWidthStartOffset] = tileType;
    }

    // 타일의 이웃들 중에서 적절한 다음 타일 위치 선택
    private bool SetNextTilePosition(int x, int z)
    {
        Vector2Int[] emptyNeighbors =
            GetNeighbors(x, z).Where(pos => _map[pos.y, pos.x] == Define.AreaTileType.Empty).ToArray();

        if (emptyNeighbors.Length == 0)
        {
            return false;
        }

        _nextTilePosition = emptyNeighbors[Random.Range(0, emptyNeighbors.Length)];
        return true;
    }

    private bool FindPath(Vector2Int destination, out List<Vector2Int> path)
    {   
        // 타일별 최단거리
        Dictionary<Vector2Int, int> distances = new()
        {
            [_playerStartPosition] = 0
        };
        Dictionary<Vector2Int, Vector2Int> previous = new();
        path = new List<Vector2Int>(); // 최단 경로 저장할 리스트
        List<Vector2Int> queue = new(){_playerStartPosition};

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

            foreach (Vector2Int neighbor in GetNeighbors(currentNode))
            {
                if (_map[neighbor.y, neighbor.x] == Define.AreaTileType.OutOfField ||
                    _map[neighbor.y, neighbor.x] == Define.AreaTileType.Obstacle) continue;

                int distanceToNeighbor = distances[currentNode] + 1;
                if (!distances.ContainsKey(neighbor) || distanceToNeighbor < distances[neighbor])
                {
                    distances[neighbor] = distanceToNeighbor;
                    previous[neighbor] = currentNode;
                    queue.Add(neighbor);
                }
            }
        }

        // 보스타일로 가는 경로가 없음
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
            List<Vector2Int> closestNodes = new() {closestNode};
            foreach (Vector2Int node in queue)
            {
                if (distances[closestNode] == distances[node])
                {
                    closestNodes.Add(node);
                }
            }
            Vector2Int selected = closestNodes[Random.Range(0, closestNodes.Count)];
            queue.Remove(selected);
            return selected;
        }
    }

    // 그리드 좌표를 월드 좌표로 변환
    private Vector3 GridToWorldPosition(int x, int z, float y = 0)
    {
        if (x % 2 == 1) return new Vector3(x * TILE_WIDTH * 0.75f, y, (z + 0.5f) * TILE_HEIGHT) + _mapOriginPosition;
        else return new Vector3(x * TILE_WIDTH * 0.75f, y, z * TILE_HEIGHT) + _mapOriginPosition;
    }

    // 월드 좌표를 그리드 좌표로 변환
    private void WorldToGridPosition(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.RoundToInt((worldPosition.x - (int)_mapOriginPosition.x) / (TILE_WIDTH * 0.75f));
        //float tempz = (worldPosition.z - (int)_mapOriginPosition.z) / TILE_HEIGHT;
        //z = x % 2 == 1 ? Mathf.RoundToInt(tempz - 0.5f) : Mathf.RoundToInt(tempz);
        z = Mathf.RoundToInt((worldPosition.z - (int)_mapOriginPosition.z) / TILE_HEIGHT - (x % 2 == 1 ? 0.5f : 0f));
    }

    private bool IsPositionValid(int x, int z)
    {
        return x >= 0 && x < _data.MapWidth && z >= 0 && z < _data.MapHeight;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        return GetNeighbors(pos.x, pos.y);
    }

    private List<Vector2Int> GetNeighbors(int x, int z)
    {
        int[,] dir = x % 2 == 0
            ? new[,] { { 0, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 } }
            : new[,] { { 0, 1 }, { 1, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 }, { -1, 1 } };

        List<Vector2Int> neighbors = new List<Vector2Int>();

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

    // 해당 셀의 이웃 6개 타일을 보며 tileType인 타일이 하나라도 있다면 true, 하나도 없다면 false 반환
    public bool HasNeighborOfType(int x, int z, Define.AreaTileType tileType)
    {
        List<Vector2Int> neighbors = GetNeighbors(x, z);
        foreach (var neighbor in neighbors)
        {
            if (_map[neighbor.y, neighbor.x] == tileType) return true;
        }

        return false;
    }
    // 타일 그룹의 부모 설정
    private Transform SetupTileGroupParent(int x, int z)
    {
        Transform tileGroupParent = new GameObject("TileGroup").transform;
        tileGroupParent.SetParent(_tileParent);
        tileGroupParent.position = GridToWorldPosition(x, z);

        return tileGroupParent;
    }

    // AreaBaseTile Init. AreaBaseTile의 Start에서 할 시 제대로 적용이 안 됨.
    private void InitBaseTiles()
    {
        for (int z = 0; z < _baseTiles.GetLength(0); z++)
        {
            for (int x = 0; x < _baseTiles.GetLength(1); x++)
            {   
                _baseTiles[z, x].Init();
            }
        }
    }

    #region Debug
    private static void ClearMap()
    {
        GameObject mapParent = GameObject.Find("@Map");

        if (mapParent != null)
        {
            for (int j = mapParent.transform.childCount - 1; j >= 0; j--)
            {
                for (int i = mapParent.transform.GetChild(j).transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(mapParent.transform.GetChild(j).GetChild(i).gameObject);
                }
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
        if (CurrentGeneratePhase == MapGeneratePhase.NotStarted)
        {
            Debug.LogWarning("Map must be generated first!");
            return;
        }

        ClearDebugObjects();

        for (int z = 0; z < _map.GetLength(0); z++)
        {
            for (int x = 0; x < _map.GetLength(1); x++)
            {
                GameObject canvas = Instantiate(_infoText, GridToWorldPosition(x, z, 2), Quaternion.Euler(60, 0, 0), _debugObjectParent);
                canvas.GetComponentInChildren<TextMeshProUGUI>().SetText(_map[z, x].ToString());
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

        for (int z = 0; z < _map.GetLength(0); z++)
        {
            for (int x = 0; x < _map.GetLength(1); x++)
            {
                GameObject canvas = Instantiate(_infoText, GridToWorldPosition(x, z, 2), Quaternion.Euler(60, 0, 0), _debugObjectParent);
                canvas.GetComponentInChildren<TextMeshProUGUI>().SetText($"{z}, {x}");
            }
        }
    }

    public void ShowPathToBoss()
    {
        if (CurrentGeneratePhase != MapGeneratePhase.PlayableFieldDecorationGenerate &&
            CurrentGeneratePhase != MapGeneratePhase.EventTileGenerate)
        {
            Debug.LogWarning("Playable Field Decoration must be generated first!");
            return;
        }

        ClearDebugObjects();
        FindPath(_bossPosition, out var path);

        foreach (var pos in path)
        {
            Vector3 position = GridToWorldPosition(pos.x, pos.y, 1.07f);
            Instantiate(_pathIndicator, position, Quaternion.Euler(90, 0, 0), _debugObjectParent);
        }
    }
    #endregion
}
