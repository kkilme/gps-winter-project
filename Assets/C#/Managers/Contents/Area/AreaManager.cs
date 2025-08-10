using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaManager
{
    public AreaMap Map { get; private set; }
    public AreaData AreaData { get; private set; }
    public Quest Quest { get; private set; } // 현재 Area의 퀘스트
    public Loot Loots { get; set; } = new(); // Area에서 획득한 아이템들
    public List<Item> Items { get; set; } = new(); // Area에서 사용하고자 가져온 아이템들: 현재는 ConsumableItem만 사용 가능

    public AreaInputHandler AreaInputHandler { get; private set; }
    public AreaCollapseSystem CollapseSystem { get; private set; }
    public AreaCameraController CameraController { get; private set; }
    public UI_AreaScene UI { get; private set; }


    public AreaState AreaState { get; set; }
    public Vector3 CurrentPlayerPosition { get; set; } // 현재 영웅 파티의 WorldPosition = 서 있는 타일의 중앙 위치
    private HeroParty _party => Managers.HeroMng.HeroParty;

    private AreaEventTile _currentTile; // 현재 파티가 밟고있는 타일
    private HashSet<Vector2Int> _visiblePositions = new(); // 파티 시야 범위 내부에 있는 위치들

    private GameObject _light;

    #region Init

    public void Init(AreaMap map, AreaInitContext areaInitContext)
    {
        Debug.Log("[AreaManager] AreaManager Init Start");

        // 각종 필드 초기화
        Map = map;
        AreaData = Managers.DataMng.AreaDataDict[areaInitContext.AreaName];
        Quest = areaInitContext.Quest;
        UI = Managers.UIMng.ShowSceneUI<UI_AreaScene>();
        foreach (var itemData in areaInitContext.Items)
        {
            Items.Add(ItemFactory.CreateItemById(itemData.DataId));
        }
        _light = GameObject.FindGameObjectWithTag("AreaLight");

        CurrentPlayerPosition = Map.GetPlayerStartWorldPosition();
        _currentTile = Map.GetEventTile(CurrentPlayerPosition);

        // AreaInputHandler 초기화
        AreaInputHandler = new AreaInputHandler();
        AreaInputHandler.Init(CurrentPlayerPosition);

        // CollapseSystem 초기화
        CollapseSystem = new AreaCollapseSystem();
        CollapseSystem.Init(AreaData.CollapseTimer, AreaData.CollapseAmount);

        // 영웅 스폰 및 시작지점에 배치
        Managers.HeroMng.SpawnHeroParty();
        PlaceHeroes(CurrentPlayerPosition);

        // 카메라 초기화
        InitCamera();

        Map.OnAreaStart(); // 일부 지역(보스 타일 등)의 전장의 안개를 미리 밝힘
        UpdateTileBrightness();
        Map.ChangeNeighborTilesColor(CurrentPlayerPosition, TileColorChangeType.Highlight);

        // 각종 초기화 완료 후, UI 초기화
        UI.OnAreaInitComplete();

        // 모든 초기화 완료 후 마우스 이벤트 핸들러 등록
        Managers.InputMng.AddMouseAction(AreaInputHandler.HandleMouseInput);

        AreaState = AreaState.Idle;

        Debug.Log("[AreaManager] AreaManager Init Complete.");
    }

    /// <summary>
    /// 영웅 파티를 targetPosition 위치의 타일 중앙을 기준으로 배치 시킴
    /// </summary>
    private void PlaceHeroes(Vector3 targetPosition)
    {
        var heroes = _party.RuntimeHeroes;
        targetPosition = Map.GetTileCenterPosition(targetPosition);
        for (int i = 0; i < heroes.Count; i++)
        {
            if (!heroes[i].IsDead()) heroes[i].transform.position = targetPosition + new Vector3(GlobalValues.HERO_POS_ON_AREA_TILE_OFFSET[i, 0], 0, GlobalValues.HERO_POS_ON_AREA_TILE_OFFSET[i, 1]);
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
        (float xmin, float xmax) = Map.CalculateCameraXPositionLimit();
        CameraController.InitPosLimit(xmin, xmax, Map.GetPlayerStartWorldPosition().z, Map.GetBossWorldPosition().z);
        CameraController.Freeze = false;
    }
    #endregion

    #region HeroMovement
    /// <summary>
    /// 대상 위치의 타일로 영웅 파티 이동
    /// </summary>
    public void MoveHeroes(Vector3 targetPosition)
    {
        if (AreaState != AreaState.Idle) return;

        // 이동 가능한 타일인지 확인
        if (!Map.IsPositionMoveable(CurrentPlayerPosition, targetPosition)) return;

        AreaState = AreaState.Moving;
        Map.ChangeNeighborTilesColor(CurrentPlayerPosition, TileColorChangeType.Reset);

        CurrentPlayerPosition = targetPosition;
        Map.RevealFogOfWar(CurrentPlayerPosition);
        UpdateTileBrightness();

        targetPosition = Map.GetTileCenterPosition(targetPosition);

        Managers.SoundMng.PlayEffect("footstep_grass");
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
        foreach (var pos in _visiblePositions)
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
    /// 현재 타일의 이벤트가 끝났을 때 호출되는 메서드
    /// </summary>
    public IEnumerator OnTileEventFinish()
    {
        if (Managers.HeroMng.HeroParty.IsAllDead())
        {
            OnQuestFailed();
            yield break;
        }

        CameraController.Freeze = false;

        switch (_currentTile.TileType)
        {
            case AreaTileType.Normal:
                break;
            case AreaTileType.Boss:
                Map.ReplaceEventTile(CurrentPlayerPosition, AreaTileType.Normal);
                OnQuestComplete();
                break;
            case AreaTileType.Battle:
            case AreaTileType.Encounter:
                Map.ReplaceEventTile(CurrentPlayerPosition, AreaTileType.Normal);
                break;
        }

        // 붕괴 턴 진행
        yield return CoroutineRunner.Instance.StartCoroutine(CollapseSystem.ProgressCollapse());

        Map.ChangeNeighborTilesColor(CurrentPlayerPosition, TileColorChangeType.Highlight);

        AreaState = AreaState.Idle;
    }
    #endregion

    #region AreaContents
    /// <summary>
    /// 파티 체력 회복 및 붕괴 턴 추가 진행
    /// </summary>
    public IEnumerator RestParty()
    {
        AreaState = AreaState.Busy;

        _party.Rest();
        yield return CoroutineRunner.Instance.StartCoroutine(CollapseSystem.ProgressCollapse(GlobalValues.AREA_REST_PROGRESS_COLLAPSE_COUNT));

        AreaState = AreaState.Idle;
    }

    /// <summary>
    /// Area에서 아이템을 사용하는 메서드.
    /// </summary>
    public void UseItem(ItemData itemData)
    {
        if (AreaState != AreaState.Idle) return;

        // itemData에 해당하는 Item을 Items 리스트에서 찾음.
        Item item = Items.Find(i => i.DataId == itemData.DataId);

        if (item == null) return; // 아이템이 Items 리스트에 존재하는지 확인

        if (item is not IUsableInArea areaItem) return; // Area에서 사용 가능한 아이템인지 확인

        areaItem.UseInArea(); // 아이템 사용
    }

    /// <summary>
    /// 이 Area에서 가능한 Encounter들 중 랜덤으로 하나를 반환
    /// </summary>
    public AreaEncounter GetRandomEncounter()
    {
        int selected = AreaData.EncounterIds.GetRandomElement();

        return Managers.ObjectHolder.Encounters[selected];
    }

    /// <summary>
    /// 이 Area에서 가능한 MonsterSquad들 중 랜덤으로 하나를 반환
    /// </summary>
    public int GetRandomMonsterSquadId()
    {
        return AreaData.MonsterSquadIds.GetRandomElement();
    }

    /// <summary>
    /// 퀘스트 완료 시 보상 지급 및 UI 팝업 표시
    /// </summary>
    private void OnQuestComplete()
    {
        Managers.SoundMng.FadeoutBGM(1f);
        Managers.SoundMng.PlayEffect("quest_complete", .4f);

        var popup = Managers.UIMng.ShowPopupUI<UI_AreaCompletePopup>();
        popup.Show(Loots, Quest);

        // 보상 지급 //
        foreach (var item in Loots.Items)
        {
            Managers.InvMng.AddItem(item.DataId);
        }
        if (!Quest.QuestData.IsComplete)
        {
            // 첫 퀘스트 클리어 보상 추가
            foreach (QuestReward reward in Quest.QuestData.FirstClearRewards)
            {
                for (int i = 0; i < reward.Quantity; i++)
                {
                    Managers.InvMng.AddItem(reward.ItemDataId);
                }
            }
            Quest.QuestData.IsComplete = true;

            // 다음 퀘스트 잠금 해제
            foreach (var questId in Quest.QuestData.UnlockQuestDataId)
            {
                QuestData questData = Managers.DataMng.QuestDataDict[questId];
                questData.IsUnlocked = true;
            }
        }
        Managers.InvMng.AddGold(Loots.Gold);
    }

    /// <summary>
    /// 퀘스트 실패 시 로직
    /// </summary>
    public void OnQuestFailed()
    {
        Managers.SoundMng.FadeoutBGM(1f);
        CameraController.Freeze = true;
        AreaInputHandler.Clear();

        UI.ShowScreenDimTop();
        Managers.UIMng.ShowPopupUI<UI_Defeat>();
    }
    #endregion

    #region SceneTransition
    /// <summary>
    /// 전투씬 로딩 시작
    /// 전투씬 전환 흐름: LoadBattleScene -> 로딩화면 Fade in 완료 -> OnBattleSceneLoadStart ->  배틀 씬 로딩 시작 및 완료 -> OnBattleSceneLoadFinish -> 로딩화면 Fade out
    /// </summary>
    public void LoadBattleScene(BattleType battleType)
    {
        AreaState = AreaState.Battle;
        Managers.SoundMng.PlayEffect("battle_start", .4f);

        CameraController.Freeze = true;
        Managers.InputMng.RemoveMouseAction(AreaInputHandler.HandleMouseInput);

        CoroutineRunner.Instance.StartCoroutine(Managers.SceneMng.LoadBattleScene(this, battleType)); // 실제 씬 로딩 시작
    }

    /// <summary>
    /// 전투씬 로딩 과정에서 로딩 UI의 Fade In 완료 시 호출
    /// </summary>
    public void OnBattleSceneLoadStart()
    {
        _light.SetActive(false);
        UI.HideInstantly();
    }

    /// <summary>
    /// 전투 씬 로딩 과정에서 전투 씬이 ActiveScene으로 전환된 후 호출
    /// </summary>
    public void OnBattleSceneLoadFinish()
    {
        CameraController.gameObject.SetActive(false);
    }

    /// <summary>
    /// 전투 씬 언로드 및 AreaScene이 ActiveScene으로 전환 된 후 호출
    /// </summary>
    public IEnumerator OnBattleSceneUnloadFinish(UI_Loading loadingUI)
    {
        PlaceHeroes(CurrentPlayerPosition); // BattleScene과 영웅 게임오브젝트를 공유하기 때문에, 다시 배치해줘야함
        CameraController.gameObject.SetActive(true);
        _light.SetActive(true);
        UI.ShowInstantly();

        yield return loadingUI.FadeOut(); // 로딩 UI Fade Out 전/후에 해야할 일이 달라서 이런 식으로 구현함

        CameraController.Freeze = false;
        Managers.InputMng.AddMouseAction(AreaInputHandler.HandleMouseInput);
        CoroutineRunner.Instance.StartCoroutine(OnTileEventFinish());
    }

    /// <summary>
    /// Town 씬 로딩 시작
    /// </summary>
    public void LoadTownScene()
    {
        CoroutineRunner.Instance.StartCoroutine(Managers.SceneMng.LoadTownScene());
    }

    #endregion

    public void Clear()
    {
        AreaInputHandler.Clear();
        Items.Clear();
    }
}