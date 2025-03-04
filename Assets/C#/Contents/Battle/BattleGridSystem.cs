using UnityEngine;

// grid 및 마우스 입력 관리
public class BattleGridSystem
{
    public BattleGridCell[,] HeroGrid { get; protected set; } = new BattleGridCell[GlobalValues.BATTLEGRID_ROW_COUNT, GlobalValues.BATTLEGRID_COL_COUNT];
    public BattleGridCell[,] MonsterGrid { get; protected set; } = new BattleGridCell[GlobalValues.BATTLEGRID_ROW_COUNT, GlobalValues.BATTLEGRID_COL_COUNT];

    private BattleManager _battleManager;

    public void Init()
    {
        GameObject heroGrid = GameObject.FindGameObjectWithTag("HeroGrid");
        GameObject monsterGrid = GameObject.FindGameObjectWithTag("MonsterGrid");

        for (int row = 0; row < GlobalValues.BATTLEGRID_ROW_COUNT; row++)
        {
            for (int col = 0; col < GlobalValues.BATTLEGRID_COL_COUNT; col++)
            {
                var herocell = GlobalUtility.FindChild(heroGrid, $"BattleGridCell ({row}, {col})");
                HeroGrid[row, col] = herocell.GetOrAddComponent<HeroBattleGridCell>();
                HeroGrid[row, col].Init(row, col, GridSide.HeroSide);

                var monsterCell = GlobalUtility.FindChild(monsterGrid, $"BattleGridCell ({row}, {col})");
                MonsterGrid[row, col] = monsterCell.GetOrAddComponent<MonsterBattleGridCell>();
                MonsterGrid[row, col].Init(row, col, GridSide.MonsterSide);
            }
        }
        _battleManager = Managers.BattleMng;
    }

    public void PlaceHero()
    {
        foreach (Hero hero in Managers.ObjectMng.HeroParty.Heroes)
        {
            Vector2Int pos = Managers.ObjectMng.HeroParty.BattlePositions[hero];
            HeroGrid[pos.y, pos.x].PlaceCreature(hero);
            hero.LookOpponent();
        }
    }

    public void PlaceEnemy(int squadId)
    {
        Managers.DataMng.MonsterSquadDataDict.TryGetValue(squadId, out Data.MonsterSquadData squadData);
        if (squadData == null)
        {
            Debug.LogError($"Failed to get MonsterSquadData, squadId: {squadId}");
            return;
        }

        foreach (Data.MonsterSquad_MonsterData monsterData in squadData.Monsters)
        {
            Monster monster = Managers.ObjectMng.SpawnMonster(monsterData.DataId);
            Vector2Int pos = new Vector2Int(monsterData.x, monsterData.y);
            MonsterGrid[pos.y, pos.x].PlaceCreature(monster);
            monster.LookOpponent();
            _battleManager.Monsters.Add(monster);
        }
    }

    public void MoveCreature(Creature creature, BattleGridCell targetCell)
    {
        // 서로의 위치 교환을 위해 밑의 if문 필요.
        // 조건이 없을 시, A를 B의 위치로 옮긴 후 B를 A의 위치로 옮길 때 문제가 생김.
        if(creature.StandingCell.PlacedCreature == creature) creature.StandingCell.PlacedCreature = null;
        targetCell.PlaceCreature(creature);
    }

    public void SwapCreaturePosition(Creature creature1, Creature creature2)
    {
        var tempCell = creature1.StandingCell;
        MoveCreature(creature1, creature2.StandingCell);
        MoveCreature(creature2, tempCell);
    }

    public void HighlightTargettableCells(BaseAction action)
    {
        var targetables = action.TargetSelector.TargettableCells;

        foreach (var targetable in targetables)
        {
            targetable.HighlightOutline();
        }
    }

    public BattleGridCell[,] SideToGrid(GridSide side)
    {
        return side == GridSide.HeroSide ? HeroGrid : MonsterGrid;
    }

    public void ResetAllCellColor()
    {
        foreach(var cell in HeroGrid)
        {
            cell.RevertFillColor();
            cell.RevertOutlineColor();
        }
        foreach (var cell in MonsterGrid)
        {
            cell.RevertFillColor();
            cell.RevertOutlineColor();
        }
    }
}
