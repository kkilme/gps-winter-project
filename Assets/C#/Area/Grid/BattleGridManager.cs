using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BattleGridManager
{
    private Dictionary<GridSide, SquareGrid> _grids;

    public SquareGrid PlayerGrid { get; set; }

    public SquareGrid EnemyGrid { get; set; }

    private Camera _mainCamera;
    // 최근 마우스 위치에 해당하는 그리드 사이드
    private GridSide _recentSide;
    // 최근 마우스 위치에 해당하는 월드 좌표
    private Vector3 _recentWorldposition;

    public Action onMouseLeftClick;
    public Action onMouseRightClick;

    private SquareGridCell _selectedCell;

    public BattleGridManager(Vector3 playerOriginPos, Vector3 enemyOriginPos)
    {   
        PlayerGrid = new SquareGrid(playerOriginPos, GridSide.Player);
        EnemyGrid = new SquareGrid(enemyOriginPos, GridSide.Enemy);
        _grids = new Dictionary<GridSide, SquareGrid> { {GridSide.Player, PlayerGrid}, { GridSide.Enemy, EnemyGrid} };
        _mainCamera = Camera.main;
    }

    public void HandleMouseHover()
    {
        bool success = TryGetGridInformation();
        if (success)
        {
            _grids[_recentSide].HandleMouseHover(_recentWorldposition);
        }
        else
        {
            _grids[_recentSide].ResetMouseHover();
        }
    }

    // 마우스 위치의 그리드 탐지. 해당 그리드의 side와 마우스 위치의 world position 얻어냄.
    public bool TryGetGridInformation()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: 100f, layerMask: LayerMask.GetMask("BattleGrid")))
        {
            GameObject grid = rayHit.transform.gameObject;

            switch (grid.tag)
            {
                case "PlayerGrid":
                    _recentSide = GridSide.Player;
                    break;
                case "EnemyGrid":
                    _recentSide = GridSide.Enemy;
                    break;
                default:
                    Debug.LogError("Grid Tag not set!");
                    break;
            }

            _recentWorldposition = rayHit.point;
            Debug.DrawLine(_mainCamera.transform.position, rayHit.point);
            return true;
        }

        return false;
    }

    // 마우스 위치의 그리드 셀 선택
    private void SelectCell()
    {
        _selectedCell = _grids[_recentSide].GetGridCell(_recentWorldposition);
    }

    public void OnBattleStateChange(BattleState from, BattleState to)
    {
        switch (from)
        {
            case BattleState.Idle:
                break;
            case BattleState.SelectingTargetPlayer:
                onMouseLeftClick -= SelectCell;
                break;
            case BattleState.SelectingTargetMonster:
                onMouseLeftClick -= SelectCell;
                break;
        }

        switch (to)
        {
            case BattleState.Idle:
                break;
            case BattleState.SelectingTargetPlayer:
                onMouseLeftClick += SelectCell;
                break;
            case BattleState.SelectingTargetMonster:
                onMouseLeftClick += SelectCell;
                break;
        }
    }
}
