using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager
{
    #region Field

    public BattleState BattleState { get; private set; }
    public UI_BattleScene BattleSceneUI { get; private set; }
    public TurnSystem TurnSystem { get; private set; }
    public BattleGridSystem BattleGridSystem { get; private set; }
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
                _currentAction.OnUnset();
                _currentAction = null;
            
            }
            else
            {
                _currentAction = value;
                _currentAction.OnSet();
            }
        }
    }
    public Creature CurrentTurnCreature => TurnSystem.Turns[0];
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
        BattleGridSystem = new BattleGridSystem();
        MouseInputHandler = new BattleMouseInputHandler();
        BattleSceneUI = Managers.UIMng.ShowSceneUI<UI_BattleScene>();
        Monsters = new();

        // 배틀 필드 생성
        string battleFieldname = Managers.DataMng.AreaDataDict[Managers.AreaMng.AreaName].BattleFieldName;
        GameObject battleField = Managers.ResourceMng.Instantiate($"Battle/Field/{battleFieldname}");
        battleField.transform.position = new Vector3(GlobalValues.BATTLEFIELD_POS_X, 0, GlobalValues.BATTLEFIELD_POS_Z);

        // MouseInputHandler 초기화 - 카메라가 배틀 필드에 포함되어 있기 때문에 반드시 배틀 필드 생성 이후에 해야함.
        MouseInputHandler.Init();

        // Creature 배치
        BattleGridSystem.Init();
        BattleGridSystem.PlaceHero();
        BattleGridSystem.PlaceEnemy(squadId);

        // TurnSystem 초기화
        TurnSystem.Init();
        
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

        BattleSceneUI.OnPlacementPhaseStart();
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

        CoroutineRunner.Instance.Run(BattleSceneUI.OnBattlePhaseStart());
    }

    // Action 선택
    public void SetAction(BaseAction action)
    {
        if(action == null)
        {
            Debug.LogError("Action is null");
            return;
        }
        CurrentAction = action;
        BattleSceneUI.BattleActionPanel.Hide();

        // 대상 선택이 필요한 액션인 경우
        if (action.TargetSelector.NeedTargetSelection)
        {
            BattleSceneUI.ChooseTargetUI.Show();

            Managers.InputMng.MouseAction -= MouseInputHandler.HandleMouseOnBattlePhase;
            Managers.InputMng.MouseAction -= MouseInputHandler.HandleMouseOnTargetSelect;
            Managers.InputMng.MouseAction += MouseInputHandler.HandleMouseOnTargetSelect;

            BattleGridSystem.HighlightTargetableCells(action);
        }
        else // 대상 선택이 필요 없는 액션인 경우
        {
            BattleGridSystem.ResetAllCellColor();
            CurrentAction.HighlightAffectedTargets();
            CoroutineRunner.Instance.Run(CurrentAction.Execute());
        }
    }

    public void UnsetAction()
    {
        CurrentAction = null;
        BattleSceneUI.ChooseTargetUI.Hide();
        BattleSceneUI.BattleActionPanel.Show();

        Managers.InputMng.MouseAction -= MouseInputHandler.HandleMouseOnTargetSelect;
        Managers.InputMng.MouseAction -= MouseInputHandler.HandleMouseOnBattlePhase;
        Managers.InputMng.MouseAction += MouseInputHandler.HandleMouseOnBattlePhase;

        BattleGridSystem.ResetAllCellColor();
        CurrentTurnCreature.StandingCell.HighlightOutline();
    }

    public void OnActionEnd()
    {
        UnsetAction();
        NextTurn();
    }

    public void NextTurn()
    {
        if (Managers.ObjectMng.Monsters.Count <= 0)
        {
            EndBattle(BattleResultType.Victory);
            return;
        }

        if (Managers.ObjectMng.Heroes.Count <= 0)
        {
            EndBattle(BattleResultType.Defeat);
            return;
        }

        TurnSystem.NextTurn();
        BattleSceneUI.TurnstateUI.MoveTurnFrames();
        

        //CurrentTurnCreature.CreatureBattleState = CreatureBattleState.PrepareAction;
        //BattleSceneUI.OnTurnStart();
    }

    public void EndBattle(BattleResultType battleResult)
    {
        ((UI_BattleScene)Managers.UIMng.SceneUI).OnBattleEnd(battleResult);
    }

}
