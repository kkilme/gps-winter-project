using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AreaManager
{
    public AreaMap Map { get; private set; }
    public Loot Loots { get; set; }
    public AreaInputHandler AreaInputHandler { get; private set; }
    public AreaCollapseSystem CollapseSystem { get; private set; }
    public AreaCameraController CameraController { get; private set; }
    public Vector3 CurrentPlayerPosition { get; set; } // 현재 플레이어 WorldPosition
    public AreaName AreaName { get; set; }

    private AreaState _areaState;
    public AreaState AreaState
    {
        get => _areaState;
        set
        {
            _areaState = value;
            if (value == AreaState.Idle)
            {
                Map.ChangeNeighborTilesColor(CurrentPlayerPosition, TileColorChangeType.Highlight);
            }
        }
    }

    private HeroParty _party => Managers.HeroMng.HeroParty;

    private AreaEventTile _currentTile; // 현재 플레이어가 밟고있는 타일
    private List<Vector2Int> _visiblePositions = new(); // 플레이어 시야 범위 내부에 있는 위치들
    private GameObject _light;

    #region Init
    public void Init(AreaMap map)
    {
        Map = map;
        Loots = new Loot();
        AreaInputHandler = new AreaInputHandler();
        CollapseSystem = new AreaCollapseSystem();
        _light = GameObject.FindGameObjectWithTag("AreaLight");

        CurrentPlayerPosition = Map.GetPlayerStartPosition();
        _currentTile = Map.GetEventTile(CurrentPlayerPosition);

        InitHeroes();
        InitCamera();
        AreaInputHandler.Init(CurrentPlayerPosition);

        Map.RevealFogOfWarOnStart();
        UpdateTileBrightness();
        AreaState = AreaState.Idle;

        Managers.InputMng.MouseAction -= AreaInputHandler.HandleMouseInput;
        Managers.InputMng.MouseAction += AreaInputHandler.HandleMouseInput;
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
        CameraController = Managers.ResourceMng.Instantiate("Area/AreaCamera").GetComponent<AreaCameraController>();
        CameraController.Freeze = true;
        CameraController.Init();
        Map.CalcCameraPosLimitX(out float xmin, out float xmax);
        CameraController.InitPosLimit(xmin, xmax, Map.GetPlayerStartPosition().z, Map.GetBossPosition().z);
        CameraController.Freeze = false;

        CameraController.transform.position = new Vector3(CurrentPlayerPosition.x, 50, CurrentPlayerPosition.z - 40);
    }
    #endregion

    /// <summary>
    /// 대상 위치의 타일로 영웅 파티 이동
    /// </summary>
    public void MoveHeroes(Vector3 targetPosition)
    {
        // 이동 가능한 타일인지 확인
        if (!Map.IsPositionMoveable(CurrentPlayerPosition, targetPosition)) return;

        AreaState = AreaState.Moving;
        Map.ChangeNeighborTilesColor(CurrentPlayerPosition, TileColorChangeType.Reset);


        CurrentPlayerPosition = targetPosition;
        Map.RevealFogOfWar(CurrentPlayerPosition);
        UpdateTileBrightness();

        targetPosition = Map.GetTileCenterPosition(targetPosition);

        Sequence moveSequence = _party.MakeMoveToSequence(targetPosition);
        _party.PlayMovingAnimation();
        moveSequence.Play().OnComplete(() =>
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

    private void UpdateTileBrightness()
    {
        var newVisiblePos = Map.GetVisibleTilePositions(CurrentPlayerPosition);
        foreach(var pos in _visiblePositions)
        {
            if (!newVisiblePos.Contains(pos))
            {
                if (Map.TileTypeMap[pos.y, pos.x] != AreaTileType.OutOfField)
                    Map.BaseTileMap[pos.y, pos.x].SetBrightness(false);
            }
        }

        foreach (var pos in newVisiblePos)
        {
            if (Map.TileTypeMap[pos.y, pos.x] == AreaTileType.OutOfField) continue;
            Map.BaseTileMap[pos.y, pos.x].SetBrightness(true);
        }

        _visiblePositions = newVisiblePos;
    }

    // 전투씬 전환 흐름: 카메라 정지 -> 로딩화면 Fade in 완료 ->  배틀 씬 로딩 시작 및 완료 -> Area의 빛, 카메라 비활성화 -> 로딩화면 Fade out
    public void LoadBattleScene()
    {
        AreaState = AreaState.Battle;
        CameraController.Freeze = true;
        Managers.InputMng.MouseAction -= AreaInputHandler.HandleMouseInput;

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

        OnTileEventFinish();

        Managers.InputMng.MouseAction -= AreaInputHandler.HandleMouseInput;
        Managers.InputMng.MouseAction += AreaInputHandler.HandleMouseInput;
    }

    public void OnTileEventFinish()
    {
        _currentTile.OnTileEventFinish();

        switch (_currentTile.TileType)
        {
            case AreaTileType.Normal:
                break;
            case AreaTileType.Battle:
                Map.CreateEventTile(CurrentPlayerPosition, AreaTileType.Normal, true);
                break;
        }

        AreaState = AreaState.Idle;
        CollapseSystem.TurnCount++;
    }
}