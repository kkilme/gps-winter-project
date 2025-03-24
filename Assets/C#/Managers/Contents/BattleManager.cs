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
    public BattleMouseInputHandler MouseInputHandler { get; private set; }
    private BaseAction _currentAction;
    public BaseAction CurrentAction {
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
            List<Creature> creatures = new(Heroes);
            creatures.AddRange(Monsters);
            return creatures;
        }
    }
    public List<Hero> Heroes;
    public List<Monster> Monsters;
    private HeroParty _party => Managers.ObjectMng.HeroParty;

    #endregion

    public void Init(int squadId)
    {
        BattleState = BattleState.Starting;
        TurnSystem = new TurnSystem();
        GridSystem = new BattleGridSystem();
        MouseInputHandler = new BattleMouseInputHandler();
        UI = Managers.UIMng.ShowSceneUI<UI_BattleScene>();
        Heroes = new(_party.Heroes);
        Monsters = new();

        // 배틀 필드 생성
        string battleFieldname = Managers.DataMng.AreaDataDict[Managers.AreaMng.AreaName].BattleFieldName;
        GameObject battleField = Managers.ResourceMng.Instantiate($"Battle/Field/{battleFieldname}");
        battleField.transform.position = new Vector3(GlobalValues.BATTLEFIELD_POS_X, 0, GlobalValues.BATTLEFIELD_POS_Z);

        // MouseInputHandler 초기화 - 카메라가 배틀 필드에 포함되어 있기 때문에 반드시 배틀 필드 생성 이후에 해야함.
        MouseInputHandler.Init();

        // Creature 배치
        GridSystem.Init();
        GridSystem.PlaceHero();
        GridSystem.PlaceMonster(squadId);

        // TurnSystem 초기화
        TurnSystem.Init();
        
        // 배치 단계 시작
        StartPlacementPhase();
    }

    // 히어로 배치 단계
    private void StartPlacementPhase()
    {   
        BattleState = BattleState.HeroPlacement;

        Managers.InputMng.MouseAction -= MouseInputHandler.HandleMouseOnPlacementPhase;
        Managers.InputMng.MouseAction += MouseInputHandler.HandleMouseOnPlacementPhase;
        Managers.InputMng.PointerOverGameObjectAction -= MouseInputHandler.OnDragEnd;
        Managers.InputMng.PointerOverGameObjectAction += MouseInputHandler.OnDragEnd;

        UI.OnPlacementPhaseStart();
    }

    // 전투 시작
    public void StartBattlePhase()
    {   
        BattleState = BattleState.Idle;

        Managers.InputMng.PointerOverGameObjectAction -= MouseInputHandler.OnDragEnd;
        Managers.InputMng.MouseAction -= MouseInputHandler.HandleMouseOnPlacementPhase;
        Managers.InputMng.MouseAction -= MouseInputHandler.HandleMouseOnBattlePhase;
        Managers.InputMng.MouseAction += MouseInputHandler.HandleMouseOnBattlePhase;

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
            UI.ChooseTargetUI.Show();

            Managers.InputMng.MouseAction -= MouseInputHandler.HandleMouseOnBattlePhase;
            Managers.InputMng.MouseAction -= MouseInputHandler.HandleMouseOnTargetSelect;
            Managers.InputMng.MouseAction += MouseInputHandler.HandleMouseOnTargetSelect;

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
        CurrentAction = null;
        UI.ChooseTargetUI.Hide();

        Managers.InputMng.MouseAction -= MouseInputHandler.HandleMouseOnTargetSelect;
        Managers.InputMng.MouseAction -= MouseInputHandler.HandleMouseOnBattlePhase;
        Managers.InputMng.MouseAction += MouseInputHandler.HandleMouseOnBattlePhase;

        GridSystem.ResetAllCellColor();
        CurrentTurnCreature.StandingCell.HighlightOutline();
    }

    public void OnActionEnd()
    {
        UnsetAction();
        UI.CoinTossDisplay.Hide();
        CoroutineRunner.Instance.StartCoroutine(NextTurn());
    }

    // 몬스터 턴 진행 로직
    public IEnumerator ProceedMonsterTurn()
    {
        Monster monster = CurrentTurnCreature as Monster;

        // AI로 스킬 선택
        CurrentAction = monster.AIBrain.DecideSkill();

        GridSystem.ResetAllCellColor();
        CurrentTurnCreature.StandingCell.HighlightOutline();
        CurrentAction.HighlightAffectedTargets();

        yield return new WaitForSeconds(1.5f);

        // 스킬 실행
        CoroutineRunner.Instance.StartCoroutine(CurrentAction.Execute());
    }

    public IEnumerator NextTurn()
    {
        UI.OnTurnEnd();
        GridSystem.ResetAllCellColor();

        yield return new WaitForSeconds(0.7f); // 턴 전환시 약간의 대기시간을 둠

        if (Monsters.Count <= 0)
        {
            EndBattle(BattleResultType.Victory);
            yield break;
        }

        if (Heroes.Count <= 0)
        {
            EndBattle(BattleResultType.Defeat);
            yield break;
        }

        TurnSystem.NextTurn();
        UI.OnTurnStart();

        if (CurrentTurnCreature is Monster)
        {
            CoroutineRunner.Instance.StartCoroutine(ProceedMonsterTurn());
        }
    }

    public void RemoveCreature(Creature creature)
    {   
        creature.StandingCell.RemoveCreature();
        TurnSystem.Remove(creature);
        UI.TurnstateUI.RemoveTurnFrame(creature);

        if (creature is Hero)
        {
            Heroes.Remove(creature as Hero);
        }
        else if (creature is Monster)
        {
            Monsters.Remove(creature as Monster);
        }

        // 현재 턴인 Creature가 이번 턴에 전투에서 이탈한 경우, 다음 턴으로 넘어감
        if (creature == CurrentTurnCreature)
        {
            CoroutineRunner.Instance.StartCoroutine(NextTurn());
        }
    }

    public void EndBattle(BattleResultType battleResult)
    {
        ((UI_BattleScene)Managers.UIMng.SceneUI).OnBattleEnd(battleResult);
    }

}
