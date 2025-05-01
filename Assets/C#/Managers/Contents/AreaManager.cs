using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AreaManager
{
    public AreaMap Map { get; private set; }
    public AreaData AreaData { get; private set; }
    public Loot Loots { get; set; }
    public AreaInputHandler AreaInputHandler { get; private set; }
    public AreaCollapseSystem CollapseSystem { get; private set; }
    public AreaCameraController CameraController { get; private set; }
    public UI_AreaScene UI { get; private set; }

    public AreaState AreaState { get; set; }

    public Vector3 CurrentPlayerPosition { get; set; } // 현재 플레이어 WorldPosition
    private HeroParty _party => Managers.HeroMng.HeroParty;

    private AreaEventTile _currentTile; // 현재 플레이어가 밟고있는 타일
    private HashSet<Vector2Int> _visiblePositions = new(); // 플레이어 시야 범위 내부에 있는 위치들
    private int _restCount = 0; // 휴식 카운트 (턴 수와는 별개로, 휴식 시마다 증가함)

    private GameObject _light;

    #region Init

    public void Init(AreaName areaName, AreaMap map)
    {
        Map = map;
        AreaData = Managers.DataMng.AreaDataDict[areaName];
        UI = Managers.UIMng.ShowSceneUI<UI_AreaScene>();
        Loots = new Loot();
        _light = GameObject.FindGameObjectWithTag("AreaLight");

        CurrentPlayerPosition = Map.GetPlayerStartWorldPosition();
        _currentTile = Map.GetEventTile(CurrentPlayerPosition);

        AreaInputHandler = new AreaInputHandler();
        AreaInputHandler.Init(CurrentPlayerPosition);

        CollapseSystem = new AreaCollapseSystem();
        CollapseSystem.Init(AreaData.CollapseTimer, AreaData.CollapseAmount);

        // 영웅 스폰 및 카메라 초기화
        InitHeroes();
        InitCamera();

        Map.OnAreaStart();
        UpdateTileBrightness();
        Map.ChangeNeighborTilesColor(CurrentPlayerPosition, TileColorChangeType.Highlight);

        UI.OnAreaInitComplete();

        Managers.InputMng.AddMouseAction(AreaInputHandler.HandleMouseInput);

        AreaState = AreaState.Idle;

        CoroutineRunner.Instance.StartCoroutine(GlobalUtility.FixUISorting(UI.gameObject)); // UI의 SortingOrder를 Fix
    }

    /// <summary>
    /// 영웅 스폰 및 위치 초기화
    /// </summary>
    private void InitHeroes()
    {
        Managers.HeroMng.SpawnHeroParty();
        var heroes = _party.Heroes;
        for (int i = 0; i < heroes.Count; i++)
        {
            heroes[i].transform.LookAt(Vector3.forward);
            heroes[i].transform.position = CurrentPlayerPosition + new Vector3(GlobalValues.HERO_POS_ON_AREA_TILE_OFFSET[i, 0], 0, GlobalValues.HERO_POS_ON_AREA_TILE_OFFSET[i, 1]);
        }
    }

    /// <summary>
    /// Area Camera 초기화
    /// </summary>
    private void InitCamera()
    {
        CameraController = Managers.ResourceMng.Instantiate("Area/AreaCamera").GetOrAddComponent<AreaCameraController>();
        CameraController.Freeze = true;
        CameraController.Init(new Vector3(CurrentPlayerPosition.x, 50, CurrentPlayerPosition.z - 40));
        Map.CalcCameraPosLimitX(out float xmin, out float xmax);
        CameraController.InitPosLimit(xmin, xmax, Map.GetPlayerStartWorldPosition().z, Map.GetBossWorldPosition().z);
        CameraController.Freeze = false;
    }
    #endregion

    /// <summary>
    /// 대상 위치의 타일로 영웅 파티 이동
    /// </summary>
    public void MoveHeroes(Vector3 targetPosition)
    {
        if(AreaState != AreaState.Idle) return;

        // 이동 가능한 타일인지 확인
        if (!Map.IsPositionMoveable(CurrentPlayerPosition, targetPosition)) return;

        AreaState = AreaState.Moving;
        Map.ChangeNeighborTilesColor(CurrentPlayerPosition, TileColorChangeType.Reset);

        CurrentPlayerPosition = targetPosition;
        Map.RevealFogOfWar(CurrentPlayerPosition);
        UpdateTileBrightness();

        targetPosition = Map.GetTileCenterPosition(targetPosition);

        _party.MoveTo(targetPosition).OnComplete(() =>
        {
            OnHeroMoved(targetPosition);
        });
    }
    
    /// <summary>
    /// 영웅 파티 이동 완료 후 실행되는 로직
    /// </summary>
    /// <param name="targetPosition"></param>
    private void OnHeroMoved(Vector3 targetPosition)
    {
        _party.StopMovingAnimation();

        _currentTile = Map.GetEventTile(CurrentPlayerPosition);
        _currentTile.OnTileEnter();
    }

    /// <summary>
    /// 시야 내/외의 타일 밝기 조정
    /// </summary>
    private void UpdateTileBrightness()
    {
        var newVisiblePos = Map.GetVisibleTilePositions(CurrentPlayerPosition);
        foreach(var pos in _visiblePositions)
        {
            if (!newVisiblePos.Contains(pos) || Map.TileTypeMap[pos.y, pos.x] == AreaTileType.Collapsed) // 붕괴된 타일도 밝기 낮춤
            {
                if (Map.TileTypeMap[pos.y, pos.x] != AreaTileType.OutOfField)
                    Map.BaseTileMap[pos.y, pos.x].SetBrightness(false);
            }
        }

        foreach (var pos in newVisiblePos)
        {
            if (!_visiblePositions.Contains(pos) && Map.TileTypeMap[pos.y, pos.x] != AreaTileType.OutOfField && Map.TileTypeMap[pos.y, pos.x] != AreaTileType.Collapsed)
                Map.BaseTileMap[pos.y, pos.x].SetBrightness(true);
        }

        _visiblePositions = newVisiblePos;
    }

    /// <summary>
    /// 영웅들 체력 회복 및 붕괴 턴 추가 진행
    /// </summary>
    public IEnumerator RestParty()
    {
        AreaState = AreaState.Busy;

        _party.Rest();
        _restCount++;
        yield return CoroutineRunner.Instance.StartCoroutine(CollapseSystem.ProgressTurn(GlobalValues.AREA_REST_TURN_COUNT));

        AreaState = AreaState.Idle;
    }

    // 전투씬 전환 흐름: 카메라 정지 -> 로딩화면 Fade in 완료 ->  배틀 씬 로딩 시작 및 완료 -> Area의 빛, 카메라 비활성화 -> 로딩화면 Fade out
    public void LoadBattleScene()
    {
        AreaState = AreaState.Battle;
        CameraController.Freeze = true;
        Managers.InputMng.RemoveMouseAction(AreaInputHandler.HandleMouseInput);

        CoroutineRunner.Instance.StartCoroutine(Managers.SceneMng.LoadBattleScene());
    }

    public void OnBattleSceneLoadFinish()
    {
        _light.SetActive(false);
        CameraController.gameObject.SetActive(false);
    }

    public void OnBattleSceneUnloadFinish(BattleResultType battleResult)
    {
        CameraController.gameObject.SetActive(true);
        CameraController.GetComponent<AreaCameraController>().Freeze = false;
        _light.SetActive(true);

        CoroutineRunner.Instance.StartCoroutine(OnTileEventFinish());

        Managers.InputMng.AddMouseAction(AreaInputHandler.HandleMouseInput);
    }

    public IEnumerator OnTileEventFinish()
    {
        _currentTile.OnTileEventFinish();

        switch (_currentTile.TileType)
        {
            case AreaTileType.Normal:
                break;
            case AreaTileType.Battle:
                Map.ReplaceEventTile(CurrentPlayerPosition, AreaTileType.Normal);
                break;
        }

        yield return CoroutineRunner.Instance.StartCoroutine(CollapseSystem.ProgressTurn());

        Map.ChangeNeighborTilesColor(CurrentPlayerPosition, TileColorChangeType.Highlight);
    }
}