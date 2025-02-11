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
    public BaseAction CurrentAction { get; private set; }
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

        // MouseInputHandler 초기화 - 반드시 배틀 필드 생성 이후에 해야함.
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
        if(BattleState != BattleState.Starting)
            return;

        BattleState = BattleState.HeroPlacement;
        Managers.InputMng.MouseAction -= MouseInputHandler.HandleMouseOnPlacementPhase;
        Managers.InputMng.MouseAction += MouseInputHandler.HandleMouseOnPlacementPhase;
        BattleSceneUI.OnPlacementPhaseStart();
    }

    // 전투 시작
    public void StartBattlePhase()
    {   
        if(BattleState != BattleState.HeroPlacement)
            return;

        BattleState = BattleState.Idle;
        Managers.InputMng.MouseAction -= MouseInputHandler.HandleMouseOnPlacementPhase;
        Managers.InputMng.MouseAction -= MouseInputHandler.HandleMouseOnBattlePhase;
        Managers.InputMng.MouseAction += MouseInputHandler.HandleMouseOnBattlePhase;
        BattleSceneUI.OnBattlePhaseStart();
    }

    public void SetAction(BaseAction action)
    {
        if (action == null) return;
        CurrentAction = action;
        CurrentAction.OnSet();
        BattleSceneUI.BattleActionPanel.Hide();
        BattleSceneUI.ChooseTargetUI.Show();
    }

    public void UnsetAction()
    {
        CurrentAction = null;
        BattleSceneUI.ChooseTargetUI.Hide();
        BattleSceneUI.BattleActionPanel.Show();
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
