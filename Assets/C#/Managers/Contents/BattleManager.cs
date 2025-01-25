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

        string battleFieldname = Managers.DataMng.AreaDataDict[Managers.AreaMng.AreaName].BattleFieldName;
        GameObject battleField = Managers.ResourceMng.Instantiate($"Battle/Field/{battleFieldname}");
        battleField.transform.position = new Vector3(Define.BATTLEFIELD_POS_X, 0, Define.BATTLEFIELD_POS_Z);

        BattleGridSystem.Init();
        BattleGridSystem.PlaceHero();
        BattleGridSystem.PlaceEnemy(squadId);

        TurnSystem.Init();
        BattleSceneUI.TurnstateUI.Setup();
        NextTurn(true);
        BattleState = Define.BattleState.Idle;
    }

    #region Battle

    public void MoveCreature(Creature creature, BattleGridCell targetCell, bool isInit = false)
    {
        if (creature.Cell != null)
            creature.Cell.PlacedCreature = null;

        targetCell.PlacedCreature = creature;
        creature.Cell = targetCell;

        if (isInit)
            creature.transform.position = targetCell.transform.position;
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
        BattleSceneUI.OnTurnStart();
    }

    public void EndBattle(Define.BattleResultType battleResult)
    {
        ((UI_BattleScene)Managers.UIMng.SceneUI).OnBattleEnd(battleResult);
    }

    #endregion
}
