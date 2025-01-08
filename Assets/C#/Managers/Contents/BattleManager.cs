using UnityEngine;

public class BattleManager
{
    #region Field
    
    public TurnSystem TurnSystem { get; private set; }
    public BattleGridSystem BattleGridSystem { get; private set; }
    private HeroParty _party => Managers.ObjectMng.HeroParty;
    public Creature CurrentTurnCreature => TurnSystem.CurrentTurnCreature();

    #endregion
    
    public void Init(int squadId)
    {
        TurnSystem = new TurnSystem();
        BattleGridSystem = new BattleGridSystem();

        string battleFieldname = Managers.DataMng.AreaDataDict[Managers.AreaMng.AreaName].BattleFieldName;
        GameObject battleField = Managers.ResourceMng.Instantiate($"Battle/Field/{battleFieldname}");
        battleField.transform.position = new Vector3(Define.BATTLEFIELD_POS_X, 0, Define.BATTLEFIELD_POS_Z);

        BattleGridSystem.Init();
        BattleGridSystem.PlaceHero();
        BattleGridSystem.PlaceEnemy(squadId);

        SetBattleTurns();
        NextTurn(true);
    }
    
    #region InitBattle

    
    public Hero PlaceHero(ulong heroId, BattleGridCell targetCell)
    {
        Hero hero = Managers.ObjectMng.Heroes[heroId];
        
        MoveCreature(hero, targetCell, true);

        return hero;
    }
    
    public Monster SpawnAndPlaceMonster(int monsterDataId, BattleGridCell targetCell)
    {
        Monster monster = Managers.ObjectMng.SpawnMonster(monsterDataId);

        Vector3 currentRotation = monster.transform.rotation.eulerAngles;
        currentRotation.y += 180f;
        monster.transform.rotation = Quaternion.Euler(currentRotation);
        
        MoveCreature(monster, targetCell, true);
        
        return monster;
    }
    
    private void SetBattleTurns()
    {
        // TODO - 속도에 따른 코드로 수정 예정
        ulong[] turns = new ulong[100];
        int turnNum = 0;
        foreach (ulong id in Managers.ObjectMng.Heroes.Keys)
            turns[turnNum++] = id;
        foreach (ulong id in Managers.ObjectMng.Monsters.Keys)
            turns[turnNum++] = id;
        
        TurnSystem.Init(turns, turnNum);
        
        Debug.Log("Current Turn: " + turns[turnNum]); // TODO - 디버깅 코드
    }
    
    #endregion

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
        }

        CurrentTurnCreature.CreatureBattleState = Define.CreatureBattleState.PrepareAction;
    }

    public void EndBattle(Define.BattleResultType battleResult)
    {
        ((UI_BattleScene)Managers.UIMng.SceneUI).EndBattle(battleResult);
    }
    
    #endregion
}
