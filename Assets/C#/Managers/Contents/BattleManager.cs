using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager
{
    #region Field

    public BattleState BattleState { get; private set; }
    public UI_BattleScene UI { get; private set; }
    public TurnSystem TurnSystem { get; private set; }
    public BattleGridSystem GridSystem { get; private set; }
    public BattleInputHandler BattleInputHandler { get; private set; }

    private BaseAction _currentAction;
    public BaseAction CurrentAction // 현재 선택된 Action
    {
        get => _currentAction;
        set
        {
            if (_currentAction == value && _currentAction.Executor == CurrentTurnCreature)
                return;

            if (value == null)
            {
                _currentAction.Unset();
                _currentAction = null;

            }
            else
            {
                _currentAction = value;
                _currentAction.Set(CurrentTurnCreature);
            }
        }
    }
    public Creature CurrentTurnCreature => TurnSystem.CurrentTurnCreature;
    public List<Creature> Creatures // 전투에 참여중인 모든 Creature
    {
        get
        {
            List<Creature> creatures = new(AliveHeroes);
            creatures.AddRange(AliveMonsters);
            return creatures;
        }
    }
    public List<Hero> AliveHeroes;
    public List<Monster> AliveMonsters;
    private HeroParty _party => Managers.HeroMng.HeroParty;

    #endregion

    public void Init(int monsterSquadId, string battleFieldName)
    {
        BattleState = BattleState.Starting;
        TurnSystem = new TurnSystem();
        GridSystem = new BattleGridSystem();
        BattleInputHandler = new BattleInputHandler();
        UI = Managers.UIMng.ShowSceneUI<UI_BattleScene>();
        AliveHeroes = new(_party.GetAliveHeroes());
        AliveMonsters = new();

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

    // 히어로 배치 단계
    private void StartPlacementPhase()
    {
        BattleState = BattleState.HeroPlacement;

        Managers.InputMng.AddMouseAction(BattleInputHandler.HandleMouseOnPlacementPhase);
        Managers.InputMng.AddPointerOverGameObjectAction(BattleInputHandler.OnDragEnd);

        UI.OnPlacementPhaseStart();
        CoroutineRunner.Instance.StartCoroutine(GlobalUtility.FixUISorting(UI.gameObject)); // UI의 SortingOrder를 Fix
    }

    // 전투 시작
    public void StartBattlePhase()
    {
        BattleState = BattleState.Idle;

        Managers.InputMng.RemovePointerOverGameObjectAction(BattleInputHandler.OnDragEnd);
        Managers.InputMng.RemoveMouseAction(BattleInputHandler.HandleMouseOnPlacementPhase);
        Managers.InputMng.AddMouseAction(BattleInputHandler.HandleMouseOnBattlePhase);

        GridSystem.ResetAllCellColor();
        CurrentTurnCreature.StandingCell.HighlightOutline();

        UI.OnBattlePhaseStart();

        if (CurrentTurnCreature is Monster)
        {
            CoroutineRunner.Instance.StartCoroutine(ProceedMonsterTurn());
        }
    }

    // Hero 턴에서 Action 선택 시
    public void SetAction(BaseAction action)
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

            GridSystem.HighlightTargettableCells(action);
        }
        else // 대상 선택이 필요 없는 액션인 경우: 대상이 스킬에서 강제로 정해져 있거나, 랜덤 대상을 선택하는 액션임
        {
            if (!action.IsExecutable())
            {
                // TODO: 선택 가능한 대상이 없을 때의 처리

            }
            action.SetRandomTarget();
            GridSystem.ResetAllCellColor();
            CurrentAction.HighlightAffectedTargets();
            CoroutineRunner.Instance.StartCoroutine(CurrentAction.Execute());
        }
    }

    public void UnsetAction()
    {
        BattleState = BattleState.Idle;
        CurrentAction = null;
        UI.ChooseTargetUI.Hide();

        Managers.InputMng.RemoveMouseAction(BattleInputHandler.HandleMouseOnTargetSelect);
        Managers.InputMng.AddMouseAction(BattleInputHandler.HandleMouseOnBattlePhase);

        GridSystem.ResetAllCellColor();
        CurrentTurnCreature.StandingCell.HighlightOutline();
    }

    public void OnActionEnd()
    {
        UnsetAction();
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
    /// <returns></returns>
    public IEnumerator NextTurn()
    {
        UI.OnTurnEnd();
        GridSystem.ResetAllCellColor();

        yield return new WaitForSeconds(0.7f); // 턴 전환시 약간의 대기시간을 둠

        if (CheckBattleFinished())
        {
            yield break;
        }

        TurnSystem.NextTurn();
        UI.OnTurnStart();

        if (CurrentTurnCreature is Monster)
        {
            CoroutineRunner.Instance.StartCoroutine(ProceedMonsterTurn());
        }
    }

    /// <summary>
    /// 전투에서 creature 제외
    /// </summary>
    /// <param name="creature"></param>
    public void RemoveCreature(Creature creature)
    {
        creature.StandingCell.RemoveCreature();
        TurnSystem.Remove(creature);
        UI.TurnstateUI.RemoveTurnFrame(creature);

        // 현재 턴인 Creature가 이번 턴에 전투에서 이탈한 경우, 다음 턴으로 넘어감
        if (creature == CurrentTurnCreature)
        {
            CoroutineRunner.Instance.StartCoroutine(NextTurn());
        }
    }

    public void RemoveHero(Hero hero, bool isFlee)
    {
        if (isFlee) UI.HeroProfileGroupUI.OnFlee(hero);
        else UI.HeroProfileGroupUI.OnDead(hero);

        AliveHeroes.Remove(hero);

        RemoveCreature(hero);
    }

    public void RemoveMonster(Monster monster)
    {
        UI.MonsterProfileGroupUI.OnDead(monster);
        AliveMonsters.Remove(monster);

        RemoveCreature(monster);
    }

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

    public void FinishBattle(BattleResultType battleResult)
    {
        Managers.InputMng.Clear();
        UI.OnBattleEnd(battleResult);
    }

    public Loot GenerateLoot()
    {
        Loot loot = new Loot();
        foreach (var monster in AliveMonsters)
        {
            loot.Add(monster.GetLoot());
        }
        return loot;
    }
}
