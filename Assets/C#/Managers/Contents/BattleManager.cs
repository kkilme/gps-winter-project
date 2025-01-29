using System.Collections.Generic;
using UnityEngine;

public class BattleManager
{
    #region Field

    public Define.BattleState BattleState { get; private set; }
    public UI_BattleScene BattleSceneUI { get; private set; }
    public TurnSystem TurnSystem { get; private set; }
    public BattleGridSystem BattleGridSystem { get; private set; }
    public Creature CurrentTurnCreature => TurnSystem.Turns[0];
    public List<Creature> Creatures
    {
        get
        {
            List<Creature> creatures = new List<Creature>(Heroes);
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
        BattleState = Define.BattleState.Starting;
        TurnSystem = new TurnSystem();
        BattleGridSystem = new BattleGridSystem();
        BattleSceneUI = Managers.UIMng.ShowSceneUI<UI_BattleScene>();
        Monsters = new();

        // 배틀 필드 생성
        string battleFieldname = Managers.DataMng.AreaDataDict[Managers.AreaMng.AreaName].BattleFieldName;
        GameObject battleField = Managers.ResourceMng.Instantiate($"Battle/Field/{battleFieldname}");
        battleField.transform.position = new Vector3(Define.BATTLEFIELD_POS_X, 0, Define.BATTLEFIELD_POS_Z);

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
        if(BattleState != Define.BattleState.Starting)
            return;

        BattleState = Define.BattleState.HeroPlacement;
        Managers.InputMng.MouseAction -= BattleGridSystem.HandleMouseInputOnPlacementPhase;
        Managers.InputMng.MouseAction += BattleGridSystem.HandleMouseInputOnPlacementPhase;
        BattleSceneUI.OnPlacementPhaseStart();
    }

    // 실제 전투 시작
    public void StartBattlePhase()
    {   
        if(BattleState != Define.BattleState.HeroPlacement)
            return;

        BattleState = Define.BattleState.Idle;
        Managers.InputMng.MouseAction -= BattleGridSystem.HandleMouseInputOnPlacementPhase;
        Managers.InputMng.MouseAction -= BattleGridSystem.HandleMouseInputOnBattlePhase;
        Managers.InputMng.MouseAction += BattleGridSystem.HandleMouseInputOnBattlePhase;
        BattleSceneUI.OnBattlePhaseStart();
    }


    public void NextTurn(bool isInit = false)
    {
        if (isInit == false)
        {
            if (Managers.ObjectMng.Monsters.Count <= 0)
            {
                EndBattle(Define.BattleResultType.Victory);
                return;
            }

            if (Managers.ObjectMng.Heroes.Count <= 0)
            {
                EndBattle(Define.BattleResultType.Defeat);
                return;
            }

            TurnSystem.NextTurn();
            BattleSceneUI.TurnstateUI.MoveTurnFrames();
        }

        //CurrentTurnCreature.CreatureBattleState = Define.CreatureBattleState.PrepareAction;
        //BattleSceneUI.OnTurnStart();
    }

    public void EndBattle(Define.BattleResultType battleResult)
    {
        ((UI_BattleScene)Managers.UIMng.SceneUI).OnBattleEnd(battleResult);
    }

}
