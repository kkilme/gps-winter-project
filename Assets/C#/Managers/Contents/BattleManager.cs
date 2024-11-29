using UnityEngine;

public class BattleManager
{
    public bool Initialized = false; // TODO - 임시 코드
    
    #region Field
    
    public TurnSystem TurnSystem { get; protected set; }
    
    public Creature CurrentTurnCreature => TurnSystem.CurrentTurnCreature();

    public BattleGridCell[,] HeroGrid { get; protected set; } = new BattleGridCell[2, 3];
    public BattleGridCell[,] MonsterGrid { get; protected set; } = new BattleGridCell[2, 3];

    #endregion
    
    public void Init()
    {
        TurnSystem = new TurnSystem();
    }
    
    #region InitBattle
    
    public void InitBattle(int monsterSquadDataId)
    {
        GameObject battleGrid = Managers.ResourceMng.Instantiate("Battle/BattleGrid", null, "@BattleGrid");
        battleGrid.transform.position = Vector3.zero;
        GameObject heroSide = Util.FindChild(battleGrid, "HeroSide");
        GameObject monsterSide = Util.FindChild(battleGrid, "MonsterSide");

        for (int row = 0; row < 2; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                HeroGrid[row, col] = Util.FindChild<BattleGridCell>(heroSide, $"BattleGridCell ({row}, {col})");
                HeroGrid[row, col].SetRowCol(row, col, Define.GridSide.HeroSide);
                MonsterGrid[row, col] = Util.FindChild<BattleGridCell>(monsterSide, $"BattleGridCell ({row}, {col})");
                MonsterGrid[row, col].SetRowCol(row, col, Define.GridSide.MonsterSide);
            }
        }

        PlaceAllCreatures(monsterSquadDataId);
        
        SetBattleTurns();
        NextTurn(true);
    }
    
    private void PlaceAllCreatures(int monsterSquadDataId)
    {
        // TODO - TEST CODE
        PlaceHero(10000, HeroGrid[0, 0]);
        //PlaceHero(10001, HeroGrid[0, 1]);
        //PlaceHero(10002, HeroGrid[1, 2]);
        
        Data.MonsterSquadData monsterSquadData = Managers.DataMng.MonsterSquadDataDict[monsterSquadDataId];
        int line1Col = 0;
        int line2Col = 0;
        foreach (int monsterId in monsterSquadData.Line1)
        {
            SpawnAndPlaceMonster(monsterId, MonsterGrid[0, line1Col++]);
        }
        foreach (int monsterId in monsterSquadData.Line2)
        {
            SpawnAndPlaceMonster(monsterId, MonsterGrid[1, line2Col++]);
        }
    }
    
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
            creature.Cell.CellCreature = null;
        
        targetCell.CellCreature = creature;
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
