using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

// XZ를 축으로 하는 그리드
// 육각형은 평평한 부분이 위 (flat-top)
public class HexGrid
{
    private int _width;

    public int Width
    {
        get => _width;
    }

    private int _height;
    public int Height
    {
        get => _height;
    }
    private float _cellwidth;
    private float _cellheight;
    private Vector3 _originPosition;

    private AreaTileType[,] _tileTypeArray;
    private HexGridCell[,] _gridArray;
    //private bool[,] _isValid;

    private HexGridCell _currentMouseoverCell;
    private GameObject _mouseoverIndicator;

    public HexGrid(int width, int height, Vector3 originposition, float cellwidth = 4, float cellheight = 3.5f)
    {
        if (width % 2 == 0)
        {
            Debug.LogError("Grid width must be odd number!");
            return;
        }
        if (height < width / 2 + 1)
        {
            Debug.LogError($"Grid height is too small! Must be equal or larger than {width / 2 + 1}!");
            return;
        }
        _width = width;
        _height = height;
        _cellwidth = cellwidth;
        _cellheight = cellheight;
        _originPosition = originposition;
        _gridArray = new HexGridCell[height, width];
        _mouseoverIndicator = Managers.ResourceMng.Instantiate("Area/mouseover_indicator");
        _mouseoverIndicator.transform.position = GetWorldPosition(width / 2, 0, 1.04f);
     
    }

    public void InitializeTileTypeArray(int[,] source)
    {
        _tileTypeArray = new AreaTileType[_height, _width];
        for (int z = 0; z < _height; z++)
        {
            for (int x = 0; x < _width; x++)
            {
                switch (source[z,x])
                {
                    case -1:
                        _tileTypeArray[z, x] = AreaTileType.Invalid;
                        break;
                    case 0:
                        _tileTypeArray[z, x] = AreaTileType.Obstacle;
                        break;
                    case 1:
                        _tileTypeArray[z, x] = AreaTileType.Empty;
                        break;
                    case 2:
                        _tileTypeArray[z, x] = AreaTileType.Start;
                        break;
                    case 3:
                        _tileTypeArray[z, x] = AreaTileType.Boss;
                        break;
                }
            }
        }
    }

    // 그리드 좌표를 월드 좌표로 변환
    public Vector3 GetWorldPosition(int x, int z, float y = 0)
    {   
        if (x % 2 == 1) return new Vector3(x * _cellwidth * 0.75f, y, (z+0.5f) * _cellheight) + _originPosition;
        else return new Vector3(x * _cellwidth * 0.75f, y, z * _cellheight) + _originPosition;
    }

    // 월드 좌표를 그리드 좌표로 변환
    public void GetGridPosition(Vector3 worldPosition, out int x, out int z)
    {

        x = Mathf.RoundToInt((worldPosition.x - (int)_originPosition.x) / (_cellwidth * 0.75f));
        float tempz = (worldPosition.z - (int)_originPosition.z) / _cellheight;

        if (x % 2 == 1)
        {
            z = Mathf.RoundToInt(tempz - 0.5f);
        }
        else
        {
            z = Mathf.RoundToInt(tempz);
        }
    }

    public void SetGridCell(int x, int z, HexGridCell gridObject)
    {
        _gridArray[z,x] = gridObject;
    }

    public void SetTileType(int x, int z, AreaTileType tileType)
    {
        _tileTypeArray[z, x] = tileType;
    }
    
    public HexGridCell GetGridCell(int x, int z)
    {
        return _gridArray[z,x];
    }

    public bool IsTileEmpty(int x, int z)
    {
        return _tileTypeArray[z, x] == AreaTileType.Empty;
    }

    // 해당 셀의 이웃 6개 타일을 보며 tileType인 타일이 하나라도 있다면 true, 하나도 없다면 false 반환
    public bool CheckNeighborType(int x, int z, AreaTileType tileType)
    {
        List<Vector2Int> neighbors = GetAllNeighbors(x, z);
        foreach (var neighbor in neighbors)
        {
            if (_tileTypeArray[neighbor.y, neighbor.x] == tileType) return true;
        }

        return false;
    }

    private bool IsPositionValid(int x, int z)
    {
        return x >= 0 && x < _width && z >= 0 && z < _height;
    }

    private bool IsPositionMoveable(int x, int z)
    {
        return _tileTypeArray[z, x] != AreaTileType.Obstacle && _tileTypeArray[z, x] != AreaTileType.Invalid;
    }

    private bool IsNeighbor(int originx, int originz, int targetx, int targetz)
    {
        List<Vector2Int> neighbors = GetAllNeighbors(originx, originz);
        foreach (var neighbor in neighbors)
        {
            if (neighbor.x == targetx && neighbor.y == targetz) return true;
        }
        return false;
    }


    public bool IsNeighbor(Vector3 origin, Vector3 target)
    {
        GetGridPosition(origin, out int x1, out int z1);
        GetGridPosition(target, out int x2, out int z2);
        return IsNeighbor(x1, z1, x2, z2);
    }

    private List<Vector2Int> GetAllNeighbors(int x, int z)
    {
        int[,] dir;
        List<Vector2Int> neighbors = new List<Vector2Int>();
        if (x % 2 == 0) dir = new[,] { { 0, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 } };
        else dir = new[,] { { 0, 1 }, { 1, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 }, { -1, 1 } };

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

    public void HandleMouseHover(Vector3 worldPosition)
    {
        GetGridPosition(worldPosition, out int x, out int z);
        //Debug.Log($"{z}, {x}");
        if (x >= 0 && x < _width && z >= 0 && z < _height)
        {
            //_currentMouseoverCell?.ChangeColor(TileColorChangeType.Reset);
            _currentMouseoverCell = _gridArray[z, x];
            _mouseoverIndicator.transform.position = GetWorldPosition(x, z, 1.07f);
            //_currentMouseoverCell?.ChangeColor(TileColorChangeType.Highlight);
        }
        else
        {
            ResetMouseHover();
        }
    }

    public void ResetMouseHover()
    {
        //_currentMouseoverCell?.ChangeColor(TileColorChangeType.Reset);
        _currentMouseoverCell = null;
    }

    public void ChangeNeighborTilesColor(Vector3 worldPosition, TileColorChangeType colorChangeType)
    {
        GetGridPosition(worldPosition, out int x, out int z);
        List<Vector2Int> neighbors = GetAllNeighbors(x, z);

        foreach (Vector2Int neighbor in neighbors)
        {
            if (IsPositionMoveable(neighbor.x, neighbor.y))
            {
                _gridArray[neighbor.y, neighbor.x].ChangeColor(colorChangeType);
            }
        }
    }

    public void OnTileEnter(Vector3 worldPosition)
    {
        GetGridPosition(worldPosition, out int x, out int z);
        GetGridCell(x, z).OnTileEnter();
    }
}

#region legacy: 유효한 그리드인지 확인해주는 2차원 bool _isValid 초기화. 맵을 미리 생성해 두는 것으로 결정되어 현재 사용하지 않음
//private void InitializeIsValid()
//{
//    _isValid = new bool[_height, _width];
//    for (int i = 0; i < _width; i++)
//    {
//        for (int j = 0; j < _height; j++)
//        {
//            _isValid[j, i] = false;
//        }
//    }

//    // flat-top hexagon, diamond-shape grid
//    int startoffset = (_width + 1) / 4;
//    int endoffset = (_width - 1) / 4;

//    for (int x = 0; x < _width; x++)
//    {
//        if (x < (_width - 1) / 2)
//        {
//            for (int z = startoffset; z < _height - endoffset; z++)
//            {
//                _isValid[z, x] = true;
//            }
//            if (x % 2 == 0) startoffset -= 1;
//            else endoffset -= 1;
//        }
//        else
//        {
//            for (int z = startoffset; z < _height - endoffset; z++)
//            {   
//                _isValid[z, x] = true;
//            }
//            if (x % 2 == 1) startoffset += 1;
//            else endoffset += 1;
//        }
//    }
#region legacy: for point-top hex, diamond-shape grid
//int startoffset = (_width - 3) / 2;
//int endoffset = (_width - 3) / 2;

//for (int z = 0; z < _height; z++)
//{
//    if (z < _width - 3)
//    {
//        for (int x = startoffset; x < _width - endoffset; x++)
//        {
//            _isValid[z, x] = true;
//        }
//        if (z % 2 == 0) startoffset -= 1;
//        else endoffset -= 1;
//    }
//    else if (_height - z <= _width - 3)
//    {
//        if (z % 2 == 0) startoffset += 1;
//        else endoffset += 1;
//        for (int x = startoffset; x < _width - endoffset; x++)
//        {
//            _isValid[z, x] = true;
//        }
//    }
//    else
//    {
//        if (z % 2 == 1) endoffset = 1;
//        else endoffset = 0;
//        for (int x = 0; x < _width - endoffset; x++)
//        {   
//            _isValid[z, x] = true;
//        }
//    }
//}
#endregion
//}
//public bool isGridPositionValid(int x, int z)
//{
//    return _isValid[z, x];
//}
#endregion

