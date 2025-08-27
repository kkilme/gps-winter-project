using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager
{
    #region Field

    public BattleState BattleState { get; private set; }
    public BattleType BattleType { get; private set; }
    public UI_BattleScene UI { get; private set; }
    public TurnSystem TurnSystem { get; private set; }
    public BattleGridSystem GridSystem { get; private set; }
    public BattleInputHandler BattleInputHandler { get; private set; }

    private BattleAction _currentAction;
    public BattleAction CurrentAction // 현재 선택된 Action
    {
        get => _currentAction;
        set
        {
            if (value == null)
            {
                if(_currentAction == null)
                    return;

                _currentAction.Unset();
                _currentAction = null;

            }
            else if(_currentAction != value || _currentAction?.Executor != CurrentTurnCreature)
            {
                _currentAction?.Unset(); // 이전 Action 해제
                _currentAction = value;
                _currentAction.Set(CurrentTurnCreature);
            }
        }
    }
    public Creature CurrentTurnCreature => TurnSystem?.CurrentTurnCreature;
    public List<Creature> AliveCreatures // 현재 전투에 참여중인 모든 Creature
    {
        get
        {
            List<Creature> creatures = new(AliveHeroes);
            creatures.AddRange(AliveMonsters);
            return creatures;
        }
    }
    public List<Hero> AliveHeroes;
    public List<Monster> Monsters; // 전투에 참여한 모든 몬스터들
    public List<Monster> AliveMonsters; // 살아있는 몬스터들


    private HeroParty _party => Managers.HeroMng.HeroParty;

    #endregion

    public void Init(int monsterSquadId, string battleFieldName, BattleType battleType)
    {
        BattleState = BattleState.Starting;
        BattleType = battleType;
        TurnSystem = new TurnSystem();
        GridSystem = new BattleGridSystem();
        BattleInputHandler = new BattleInputHandler();
        UI = Managers.UIMng.ShowSceneUI<UI_BattleScene>();
        AliveHeroes = new(_party.GetAliveHeroes());
        AliveMonsters = new();
        Monsters = new();

        // 배틀 필드 생성
        GameObject battleField = Managers.ResourceMng.Instantiate(GlobalValues.BATTLEFIELD_PATH_PREFIX + battleFieldName, new Vector3(GlobalValues.BATTLEFIELD_POS_X, 0, GlobalValues.BATTLEFIELD_POS_Z));

        // BattleInputHandler 초기화 - 카메라가 배틀 필드에 포함되어 있기 때문에 반드시 배틀 필드 생성 이후에 해야함.
        BattleInputHandler.Init();

        // GridSystem 초기화 및 Creature 배치
        GridSystem.Init();
        GridSystem.PlaceHero(AliveHeroes);
        GridSystem.PlaceMonster(monsterSquadId);

        // TurnSystem 초기화
        TurnSystem.Init();

        // 배치 단계 시작
        StartPlacementPhase();
    }

    /// <summary>
    /// 영웅 배치 단계 시작
    /// </summary>
    private void StartPlacementPhase()
    {
        Managers.InputMng.AddMouseAction(BattleInputHandler.HandleMouseOnPlacementPhase);
        Managers.InputMng.AddPointerOverGameObjectAction(BattleInputHandler.OnDragEnd_PlacementPhase);

        UI.OnPlacementPhaseStart();
        BattleState = BattleState.HeroPlacement;
    }

    /// <summary>
    /// 전투 시작
    /// </summary>
    public void StartBattlePhase()
    {
        BattleState = BattleState.Idle;

        Managers.InputMng.AddMouseAction(BattleInputHandler.HandleMouseOnBattlePhase);

        GridSystem.ResetAllCellColor();
        CurrentTurnCreature.StandingCell.HighlightOutline();

        UI.OnBattlePhaseStart();

        if (CurrentTurnCreature is Monster)
        {
            CoroutineRunner.Instance.StartCoroutine(ProceedMonsterTurn());
        }
    }

    /// <summary>
    /// Hero 턴에서 플레이어가 Action 선택 시 호출
    /// </summary>
    public void SetAction(BattleAction action)
    {
        CurrentAction = action;
        UI.ActionPanel.Hide();

        // 유저가 직접 대상을 선택할 필요가 있는 액션인 경우
        if (action.TargetSelector.NeedTargetSelection)
        {
            BattleState = BattleState.ActionTargetSelecting;
            UI.ChooseTargetUI.Show();

            Managers.InputMng.RemoveMouseAction(BattleInputHandler.HandleMouseOnBattlePhase);
            Managers.InputMng.AddMouseAction(BattleInputHandler.HandleMouseOnTargetSelect);

            GridSystem.HighlightTargetableCells(action);
            // 선택 가능한 대상이 없을 때 플레이어에게 알리는 로직이 필요할지도
        }
        else // 대상 선택이 필요 없는 액션인 경우: 대상이 액션에서 강제로 정해져 있거나, 랜덤 대상을 선택하는 액션임
        {
            if (!action.IsExecutable())
            {
                UnsetAction();
                // 선택 가능한 대상이 없을 때 플레이어에게 알리는 로직이 필요할지도
            }
            action.SetRandomTarget();
            GridSystem.ResetAllCellColor();
            CurrentAction.HighlightAffectedTargets();
            CoroutineRunner.Instance.StartCoroutine(CurrentAction.Execute());
        }
    }

    public void SetAction(ItemAction itemAction)
    {
        UI.CoinTossDisplay.HideInstantly(); // 아이템 사용은 코인 토스를 하지 않으므로 숨김
        SetAction(action: itemAction);
    }

    /// <summary>
    /// Action 실행이 끝나거나, Cancel 되었을 때 호출
    /// </summary>
    public void UnsetAction()
    {
        BattleState = BattleState.Idle;
        CurrentAction = null;
        UI.ChooseTargetUI.Hide();
        UI.ActionPanel.Show();

        Managers.InputMng.RemoveMouseAction(BattleInputHandler.HandleMouseOnTargetSelect);
        Managers.InputMng.AddMouseAction(BattleInputHandler.HandleMouseOnBattlePhase);

        GridSystem.ResetAllCellColor();
        CurrentTurnCreature.StandingCell.HighlightOutline();
    }

    /// <summary>
    /// Action 실행이 끝났을 때 호출
    /// </summary>
    public void OnActionEnd()
    {
        UnsetAction();
        UI.ActionPanel.Hide();
        UI.CoinTossDisplay.Hide();
        CoroutineRunner.Instance.StartCoroutine(NextTurn());
    }

    /// <summary>
    /// 몬스터 턴 진행 로직
    /// </summary>
    public IEnumerator ProceedMonsterTurn()
    {
        Monster monster = CurrentTurnCreature as Monster;

        // AI로 스킬 선택
        CurrentAction = monster.AIBrain.DecideSkill();

        GridSystem.ResetAllCellColor();
        CurrentTurnCreature.StandingCell.HighlightOutline();
        CurrentAction.HighlightAffectedTargets();

        yield return new WaitForSeconds(1.5f);

        BattleState = BattleState.ActionProcessing;
        // 스킬 실행
        CoroutineRunner.Instance.StartCoroutine(CurrentAction.Execute());
    }

    /// <summary>
    /// 전투 종료 조건 확인 및 다음 턴으로 진행
    /// </summary>
    public IEnumerator NextTurn()
    {
        UI.OnTurnEnd();
        GridSystem.ResetAllCellColor();

        yield return new WaitForSeconds(0.7f); // 턴 전환시 약간의 대기시간을 둠

        if (CheckBattleFinished()) // 전투가 종료되었는지 확인
        {
            yield break;
        }

        TurnSystem.NextTurn();
        UI.OnTurnStart();

        if (CurrentTurnCreature is Monster)
        {
            CoroutineRunner.Instance.StartCoroutine(ProceedMonsterTurn()); // 몬스터 턴일 시 몬스터 턴 진행
        }
    }

    /// <summary>
    /// 전투에서 creature 제외
    /// </summary>
    public void RemoveCreature(Creature creature)
    {
        creature.StandingCell.RemoveCreature();
        TurnSystem.Remove(creature);
        UI.TurnStateUI.RemoveTurnFrame(creature);

        // 현재 턴인 Creature가 이번 턴에 전투에서 이탈한 경우, 다음 턴으로 넘어감
        if (creature == CurrentTurnCreature)
        {
            CoroutineRunner.Instance.StartCoroutine(NextTurn());
        }
    }

    /// <summary>
    /// 전투에서 hero 제외 (죽거나 도망친 경우)
    /// </summary>
    public void RemoveHero(Hero hero, bool isFlee)
    {
        if (isFlee) UI.HeroProfileGroupUI.OnFlee(hero); // 도망친 경우
        else// 죽은 경우
        {
            Managers.AreaMng.UI.HeroProfileGroupUI.OnDead(hero); // Area UI에도 반영
            UI.HeroProfileGroupUI.OnDead(hero);
        }

        AliveHeroes.Remove(hero);

        RemoveCreature(hero);
    }

    /// <summary>
    /// 전투에서 monster 제외 (죽은 경우)
    /// </summary>
    public void RemoveMonster(Monster monster)
    {
        UI.MonsterProfileGroupUI.OnDead(monster);
        AliveMonsters.Remove(monster);

        RemoveCreature(monster);
    }

    /// <summary>
    /// 전투가 종료되었는지 확인 및 종료여부 반환.
    /// </summary>
    public bool CheckBattleFinished()
    {
        if (AliveMonsters.Count <= 0)
        {
            FinishBattle(BattleResultType.Victory);
            return true;
        }
        if (_party.IsAllDead())
        {
            FinishBattle(BattleResultType.Defeat);
            return true;
        }
        if (AliveHeroes.Count <= 0)
        {
            FinishBattle(BattleResultType.Retreat);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 전투 결과에 따른 전투 종료 처리.
    /// </summary>
    public void FinishBattle(BattleResultType battleResult)
    {
        Managers.SoundMng.FadeoutBGM();
        UI.OnBattleEnd(battleResult);
    }

    /// <summary>
    /// 전투한 몬스터들로부터 전리품 생성 및 반환.
    /// </summary>
    public Loot GenerateLoot()
    {
        Loot loot = new Loot();
        foreach (var monster in Monsters)
        {
            loot.Add(monster.GetLoot());
        }
        return loot;
    }

    /// <summary>
    /// 전투 결과에 따른 배틀 씬 언로드 시작
    /// </summary>
    public void UnloadBattleScene(BattleResultType battleResult)
    {
        Clear();
        CoroutineRunner.Instance.StartCoroutine(Managers.SceneMng.EndBattleScene(battleResult));
    }

    /// <summary>
    /// 전투 종료 후 BattleManager 초기화
    /// </summary>
    public void Clear()
    {
        BattleInputHandler?.Clear();
        GridSystem?.Clear();
        TurnSystem?.Clear();

        BattleInputHandler = null;
        GridSystem = null;
        TurnSystem = null;

        CurrentAction?.Unset();
        _currentAction = null;

        AliveHeroes?.Clear();
        AliveHeroes = null;

        AliveMonsters?.Clear();
        AliveMonsters = null;

        foreach (var monster in Monsters)
        {
            Managers.ResourceMng.Destroy(monster.gameObject);
        }
        Monsters?.Clear();
        Monsters = null;

        UI?.HideInstantly();
        UI = null;
    }
}
