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
            if (_currentAction == value)
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
    public List<Creature> Creatures
    {
        get
        {
            List<Creature> creatures = new(Heroes);
            creatures.AddRange(Monsters);
            return creatures;
        }
    }
    public List<Hero> Heroes => _party.Heroes;
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

        CurrentTurnCreature.StandingCell.HighlightOutline();

        UI.OnBattlePhaseStart();

        if (CurrentTurnCreature is Monster)
        {
            CoroutineRunner.Instance.StartCoroutine(ProceedMonsterTurn());
        }
    }

    // Hero가 Action 선택 시
    public void SetAction_Hero(BaseAction action)
    {
        if(action == null)
        {
            Debug.LogError("Action is null");
            return;
        }
        CurrentAction = action;
        UI.ActionPanel.Hide();

        // 대상 선택이 필요한 액션인 경우
        if (action.TargetSelector.NeedTargetSelection)
        {
            UI.ChooseTargetUI.Show();

            Managers.InputMng.MouseAction -= MouseInputHandler.HandleMouseOnBattlePhase;
            Managers.InputMng.MouseAction -= MouseInputHandler.HandleMouseOnTargetSelect;
            Managers.InputMng.MouseAction += MouseInputHandler.HandleMouseOnTargetSelect;

            GridSystem.HighlightTargettableCells(action);
        }
        else if(action.IsExecutable()) // 대상 선택이 필요 없는 액션인 경우
        {
            action.SetRandomTarget();
            GridSystem.ResetAllCellColor();
            CurrentAction.HighlightAffectedTargets();
            CoroutineRunner.Instance.StartCoroutine(CurrentAction.Execute());
        } else
        {
            Debug.LogWarning("Action is not executable");
            UnsetAction();
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

    public IEnumerator ProceedMonsterTurn()
    {
        Monster monster = CurrentTurnCreature as Monster;
        CurrentAction = monster.AIBrain.DecideSkill();

        GridSystem.ResetAllCellColor();
        CurrentTurnCreature.StandingCell.HighlightOutline();
        CurrentAction.HighlightAffectedTargets();

        yield return new WaitForSeconds(1.5f);

        CoroutineRunner.Instance.StartCoroutine(CurrentAction.Execute());
    }

    public IEnumerator NextTurn()
    {
        UI.OnTurnEnd();
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

    public void EndBattle(BattleResultType battleResult)
    {
        ((UI_BattleScene)Managers.UIMng.SceneUI).OnBattleEnd(battleResult);
    }

}
