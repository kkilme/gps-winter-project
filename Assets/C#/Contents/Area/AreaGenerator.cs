using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static Define;


public class AreaGenerator
{
    private GameObject _map;
    private AreaName _areaName;
    private int Width
    {
        get => _grid.Width;
    }
    private int Height
    {
        get => _grid.Height;
    }
    private Vector3 _originPosition;

    private AreaGrid _grid;

    public AreaGrid Grid
    {
        get => _grid;
    }

    private Transform _tileParent;

    private int _battleTileNum;
    private int _encounterTileNum;

    private const string TEST_GRID_POSITION_TEXT_PATH = "Area/TestPositionText";
    

    public AreaGenerator(AreaName areaName, Vector3 originPosition)
    {
        _areaName = areaName;
        _originPosition = originPosition;
        _grid = new AreaGrid(Managers.DataMng.AreaDataDict[_areaName].Width, Managers.DataMng.AreaDataDict[_areaName].Height, originPosition);
        _grid.InitializeTileTypeArray(ParseBasemap(Managers.DataMng.AreaDataDict[_areaName].Basemap));
        //Debug.Log(_basemap);
        _battleTileNum = Managers.DataMng.AreaDataDict[_areaName].BattleTileNum;
        _encounterTileNum = Managers.DataMng.AreaDataDict[_areaName].EncounterTileNum;

        _tileParent = new GameObject("Tiles").transform;
    }

    // 맵 생성 로직 흐름 관리
    public void GenerateMap()
    {
        InstantiateBaseMap(Managers.DataMng.AreaDataDict[_areaName].MapPrefabPath);
        GenerateStartTile();
        GenerateBossTile();
        GenerateBattleTile();
        GenerateEncounterTile();
        GenerateNormalTile();
    }

    // 일반 타일
    private void GenerateNormalTile()
    {
        for (int z = 0; z < Height; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (_grid.IsTileEmpty(x, z))
                {
                    CreateTile(x, z, AreaTileType.Normal);
                }
            }
        }
    }

    private void GenerateBattleTile()
    {
        GenerateTileWithWindow(2, _battleTileNum, AreaTileType.Battle);
    }

    private void GenerateEncounterTile()
    {
        GenerateTileWithWindow(2, _encounterTileNum, AreaTileType.Encounter);
    }

    // 시작 타일: 맨 밑 중간
    private void GenerateStartTile()
    {
        CreateTile(Width / 2, 0, AreaTileType.Start);
    }

    // 보스 타일: 맨 위 중간
    private void GenerateBossTile()
    {
        CreateTile(Width / 2, Height-1, AreaTileType.Boss);
    }

    private void InstantiateBaseMap(string path)
    {
        GameObject map = Managers.ResourceMng.Instantiate(path);
        map.transform.position = _tileParent.transform.position = _originPosition;
        _map = map;
        _tileParent.transform.SetParent(_map.transform);
    }

    // for test: 그리드 셀 위에 해당 셀의 좌표 나타내는 텍스트 띄움
    private void InstantiateGridPositionText(int x, int z)
    {
        GameObject canvas = Managers.ResourceMng.Instantiate(TEST_GRID_POSITION_TEXT_PATH, _tileParent);
        canvas.GetComponentInChildren<TextMeshProUGUI>().SetText($"{z}, {x}");
        canvas.transform.position = _grid.GetWorldPosition(x, z, 2);
        canvas.transform.rotation = Quaternion.Euler(60, 0, 0);
    }

    private void CreateTile(int x, int z, AreaTileType tileType)
    {
        Vector3 worldPosition = _grid.GetWorldPosition(x, z, 1.02f);

        AreaGridTile tile = TileFactory.CreateTile(worldPosition, tileType);

        _grid.SetTile(x, z, tile);
        _grid.SetTileType(x, z, tileType);
        // for test  ////////
        //InstantiateGridPositionText(x, z);
        /////////////////////
    }

    private void GenerateTileWithWindow(int windowSize, int tilenum, AreaTileType tileType)
    {
        int[] windowStartArr = new int[Height - windowSize - 2];
        for (int i = 2; i < Height - windowSize; i++)
        {
            windowStartArr[i - 2] = i;
        }
        int count = 0;

        while (count < tilenum)
        {
            // 모든 windowstart가 한번씩 다 선택됐다면 다시 초기화
            if (windowStartArr.Length == 0)
            {
                windowStartArr = new int[Height - windowSize - 1];
                for (int i = 2; i < Height - windowSize; i++)
                {
                    windowStartArr[i - 2] = i;
                }
            }
            int x = 0, z = 0;
            int trycnt = 0;
            bool selected = false;
            while (!selected)
            {
                trycnt++;
                // z 좌표: 랜덤으로 선택된 windowstart를 시작으로 windowsize만큼의 범위에서 랜덤 선택
                int windowstart = windowStartArr[UnityEngine.Random.Range(0, windowStartArr.Length)];
                z = UnityEngine.Random.Range(windowstart, windowstart + windowSize - 1);
                // x 좌표는 그냥 width 범위에서 랜덤
                x = UnityEngine.Random.Range(0, Width);
                // 빈 타일이어야 하며, 근처 1칸 범위에 같은 종류 타일이 없어야 함
                if (_grid.IsTileEmpty(x, z) && !_grid.CheckNeighborType(x, z, tileType)) selected = true;
                if (trycnt == 100)
                {   
                    // 해당 z좌표에 더 생성할 수 없음 -> all random 시도
                    GenerateTileWithAllRandom(tileType);
                    break;
                }
            }

            CreateTile(x, z, tileType);
            count++;

            // 한 번 선택된 windowstart는 다시 선택되지 않음 -> 한 곳에 타일이 몰리는 것을 방지
            windowStartArr = windowStartArr.Where(n => n != z).ToArray();
        }
    }

    private void GenerateTileWithAllRandom(AreaTileType tileType)
    {
        int x = 0, z = 0;
        bool selected = false;
        int trycnt = 0;
        while (!selected)
        {
            trycnt++;
            // z 좌표: 시작 지점 + 1칸, 보스 타일 제외한 height 범위에서 랜덤
            z = UnityEngine.Random.Range(2, Height - 2);
            // x 좌표: width 범위에서 랜덤
            x = UnityEngine.Random.Range(0, Width);
            if (_grid.IsTileEmpty(x, z) && !_grid.CheckNeighborType(x, z, tileType)) selected = true;
            if (trycnt == 100)
            {
                Debug.LogError("Could not select tile position with all random!");
                Debug.Log($"{z}, {x}");
                return;
            }
        }
        CreateTile(x, z, tileType);
    }

    private int[,] ParseBasemap(string input)
    {
        int[,] basemap = new int[Height, Width];

        // 문자열을 행으로 분리
        string[] rows = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);

        // 각 요소를 배열에 배치
        for (int i = 0; i < Height; i++)
        {
            string[] values = rows[i].Split(",", StringSplitOptions.RemoveEmptyEntries);

            for (int j = 0; j < Width; j++)
            {
                basemap[i, j] = int.Parse(values[j].Trim());
            }
        }

        return basemap;
    }
}
