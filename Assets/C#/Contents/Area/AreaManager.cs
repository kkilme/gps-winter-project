using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class AreaManager
{   
    public Define.AreaName AreaName { get; set; }

    private Define.AreaState _areaState;

    public Define.AreaState AreaState
    {
        get => _areaState;
        set
        {
            _areaState = value;
            if (value == Define.AreaState.Idle)
            {
                _map.ChangeNeighborTilesColor(_currentPlayerPosition, TileColorChangeType.Highlight);
            }
        }
    }

    private AreaMap _map;

    private GameObject _mouseoverIndicator; // 마우스 오버 위치의 타일 강조하는 게임오브젝트
    private GameObject _player;
    private Vector3 _currentPlayerPosition; // 현재 플레이어 WorldPosition
    private Vector3 _currentMouseoverPosition; // 현재 마우스 오버 위치의 WorldPosition
    private AreaEventTile _currentTile; // 현재 플레이어가 밟고있는 타일

    private AreaCameraController _cameraController;
    private GameObject _light;

    private int _turnCount = 0;
    public int TurnCount
    {
        get => _turnCount;
        set
        {
            _turnCount = value;
            if (TurnCount != 0 && TurnCount % _suddendeathTimer == 0) HandleSuddendeath();
            else AreaState = Define.AreaState.Idle;
        }
    }
    private int _suddendeathTimer = 4; // timer번의 이동마다 맨 밑 타일 파괴됨. Area별로 다르게 할 수도?
    private int _suddendeathCount = 0;
    
    #region Init
    public void Init(AreaMap map, bool isTest=false)
    {
        _map = map;

        if (isTest)
        {
            SpawnPlayersOnTest();
        }

        SpawnPlayer();
        InitCamera();

        Managers.InputMng.MouseAction -= HandleMouseInput;
        Managers.InputMng.MouseAction += HandleMouseInput;

        AreaState = Define.AreaState.Idle;
        _light = GameObject.FindGameObjectWithTag("AreaLight");
        _mouseoverIndicator = Managers.ResourceMng.Instantiate("Area/mouseover_indicator");
        _mouseoverIndicator.transform.position = _map.GetPlayerStartPosition();
    }

    private void SpawnPlayer()
    {
        Vector3 spawnOriginPos = _map.GetPlayerStartPosition();
        _currentPlayerPosition = spawnOriginPos;
        _currentTile = _map.GetEventTile(_currentPlayerPosition);

        _player = Managers.ObjectMng.Heroes.Values.ToList()[0].gameObject;
        _player.transform.position = spawnOriginPos;
    }

    private void InitCamera()
    {
        _cameraController = Managers.ResourceMng.Instantiate("Area/AreaCamera").GetComponent<AreaCameraController>();
        _cameraController.Freeze = true;
        _cameraController.Init();
        _map.CalcCameraPosLimitX(out float xmin, out float xmax);
        _cameraController.InitPosLimit(xmin, xmax, _map.GetPlayerStartPosition().z, _map.GetBossPosition().z);
        _cameraController.Freeze = false;

        _cameraController.transform.position = new Vector3(_currentPlayerPosition.x, 50, _currentPlayerPosition.z - 40);
    }
    #endregion

    private void HandleMouseInput(Define.MouseEvent mouseEvent)
    {   
        if (AreaState != Define.AreaState.Idle)
        {
            return;
        }

        if (_cameraController.GetMouseoverPosition(out Vector3 mouseOverPosition) && _map.IsPositionStandable(mouseOverPosition))
        {
            _mouseoverIndicator.transform.position = _map.GetTileCenterPosition(mouseOverPosition);
            _currentMouseoverPosition = mouseOverPosition;
        } else return;

        switch (mouseEvent)
        {
            case Define.MouseEvent.PointerUp:
                MovePlayers(_currentMouseoverPosition);
                break;
        }
    }

    #region Explore
    private void MovePlayers(Vector3 destination)
    {   
        // 이동 가능한 타일인지 확인
        if (_map.IsPositionMoveable(_currentPlayerPosition, _currentMouseoverPosition))
        {
            AreaState = Define.AreaState.Moving;
            _map.ChangeNeighborTilesColor(_currentPlayerPosition, TileColorChangeType.Reset);
        }
        else return;

        destination = _map.GetTileCenterPosition(destination);

        Sequence moveSequence = DOTween.Sequence();

    
        _player.transform.LookAt(destination);
        moveSequence.Join(_player.transform.DOMove(destination, 0.7f));
        
        moveSequence.Play().OnComplete(() =>
        {
            _currentPlayerPosition = destination;
            _currentTile = _map.GetEventTile(destination);
            _currentTile.OnTileEnter();
        });
    }

    // 전투씬 전환 흐름: 카메라 정지 -> 로딩화면 Fade in 완료 -> Area의 빛, 카메라 비활성화 -> 배틀 씬 로딩 시작 및 완료 -> 로딩화면 Fade out
    public IEnumerator LoadBattleScene()
    {
        var sceneName = "TestBattleScene"; // TODO - Test Code

        _cameraController.GetComponent<AreaCameraController>().Freeze = true; // 카메라 정지

        UI_Loading loadingScreen = Managers.UIMng.ShowSceneUI<UI_Loading>();

        yield return loadingScreen.Fade(true); // fade in

        _light.SetActive(false);
        _cameraController.gameObject.SetActive(false);

        var battleScene = SceneManager.GetSceneByName(sceneName);
        if (!battleScene.isLoaded)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        yield return loadingScreen.Fade(false); // fade out, LoadingUI 삭제
    }

    public void OnBattleSceneUnloadFinish()
    {   
        // TODO - 배틀 씬에서 에어리어 씬으로 넘어올 시 배틀 씬 UI 삭제하는 코드. 구조적으로 더 좋은 코드가 가능해 보임.
        GameObject.Destroy(GameObject.FindObjectOfType<UI_BattleScene>().gameObject);

        _cameraController.gameObject.SetActive(true);
        _cameraController.GetComponent<AreaCameraController>().Freeze = false;
        _light.SetActive(true);
        
        OnTileEventFinish();
    }

    public void OnTileEventFinish()
    {
        switch (_currentTile.TileType)
        {
            case Define.AreaTileType.Normal:
                break;
            case Define.AreaTileType.Battle:
                _currentTile.OnTileEventFinish();
                _map.CreateEventTile(_currentPlayerPosition, Define.AreaTileType.Normal, true);
                break;
        }
        Managers.AreaMng.TurnCount++;
    }

    private void HandleSuddendeath()
    {
        // 보스 위치 기준 최대 2칸 아래까지만 파괴됨
        if (_suddendeathCount == _map.BossPosition.y - _map.PlayerStartPosition.y - 2) return;

        _map.OnSuddendeath(_suddendeathCount);
        _suddendeathCount++;
        AreaState = Define.AreaState.Idle;
    }
    #endregion

    private void SpawnPlayersOnTest()
    {
        List<GameObject> heroes = new List<GameObject>
        {
            Managers.ObjectMng.SpawnHero(Define.HERO_KNIGHT_ID).gameObject,
            Managers.ObjectMng.SpawnHero(Define.HERO_KNIGHT_ID).gameObject,
            Managers.ObjectMng.SpawnHero(Define.HERO_WIZARD_ID).gameObject,
            Managers.ObjectMng.SpawnHero(Define.HERO_WIZARD_ID).gameObject
        };

        for (int i = 1; i < heroes.Count; i++)
        {
            heroes[i].transform.position = _map.GridToWorldPosition(0, 0, 1.04f);
            heroes[i].SetLayerRecursively(LayerMask.NameToLayer("AreaInvisiblePlayer"));
        }

    }
}