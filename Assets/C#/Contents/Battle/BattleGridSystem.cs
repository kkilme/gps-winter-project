using UnityEngine;

// grid 및 마우스 입력 관리
public class BattleGridSystem
{
    public BattleGridCell[,] HeroGrid { get; protected set; } = new BattleGridCell[2, 3];
    public BattleGridCell[,] MonsterGrid { get; protected set; } = new BattleGridCell[2, 3];

    private BattleManager _battleManager;


    public void Init()
    {
        GameObject heroGrid = GameObject.FindGameObjectWithTag("HeroGrid");
        GameObject monsterGrid = GameObject.FindGameObjectWithTag("MonsterGrid");

        for (int row = 0; row < 2; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                var herocell = Util.FindChild(heroGrid, $"BattleGridCell ({row}, {col})");
                HeroGrid[row, col] = herocell.GetOrAddComponent<HeroBattleGridCell>();
                HeroGrid[row, col].Init(row, col, GridSide.HeroSide);

                var monsterCell = Util.FindChild(monsterGrid, $"BattleGridCell ({row}, {col})");
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
        if(creature.CurrentCell.PlacedCreature == creature) creature.CurrentCell.PlacedCreature = null;
        targetCell.PlaceCreature(creature);
        //creature.transform.LookAt(HeroGrid[targetCell.Row, 2 - targetCell.Column].transform.position);
    }

    public void HighlightTargetableCells(BaseAction action)
    {
        var targetables = action.TargetSelector.TargettableCells;

        foreach (var targetable in targetables)
        {
            targetable.HighlightOutline();
        }
    }

    public void ResetCellColor()
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
