using System.Collections.Generic;
using UnityEngine;

// 전투 씬의 grid 관련 로직 관리
public class BattleGridSystem
{
    public BattleGridCell[,] HeroGrid { get; protected set; } = new BattleGridCell[GlobalValues.BATTLEGRID_ROW_COUNT, GlobalValues.BATTLEGRID_COL_COUNT];
    public BattleGridCell[,] MonsterGrid { get; protected set; } = new BattleGridCell[GlobalValues.BATTLEGRID_ROW_COUNT, GlobalValues.BATTLEGRID_COL_COUNT];

    private BattleManager _battleManager => Managers.BattleMng;

    public void Init()
    {
        GameObject heroGrid = GameObject.FindGameObjectWithTag("HeroGrid");
        GameObject monsterGrid = GameObject.FindGameObjectWithTag("MonsterGrid");

        for (int row = 0; row < GlobalValues.BATTLEGRID_ROW_COUNT; row++)
        {
            for (int col = 0; col < GlobalValues.BATTLEGRID_COL_COUNT; col++)
            {
                // 현재 Grid와 각 셀들을 미리 만들어두고 사용중임.
                // 그리드 오브젝트를 동적으로 생성해야 할 시 수정 필요.
                var herocell = GlobalUtility.FindChild(heroGrid, $"BattleGridCell ({row}, {col})");
                var monsterCell = GlobalUtility.FindChild(monsterGrid, $"BattleGridCell ({row}, {col})");
                if(herocell == null || monsterCell == null)
                {
                    Debug.LogError($"Failed to find BattleGridCell ({row}, {col})");
                    continue;
                }
                HeroGrid[row, col] = herocell.GetOrAddComponent<HeroBattleGridCell>();
                HeroGrid[row, col].Init(row, col, GridSide.HeroSide);

                MonsterGrid[row, col] = monsterCell.GetOrAddComponent<MonsterBattleGridCell>();
                MonsterGrid[row, col].Init(row, col, GridSide.MonsterSide);
            }
        }
    }

    /// <summary>
    /// 저장된(또는 초기) 그리드 위치에 Hero 배치
    /// </summary>
    public void PlaceHero(List<Hero> heroes)
    {
        foreach (Hero hero in heroes)
        {
            Vector2Int pos = Managers.HeroMng.HeroParty.GetBattlePosition(hero.InstanceId);
            HeroGrid[pos.y, pos.x].PlaceCreature(hero);
            hero.LookFront();
        }
    }

    /// <summary>
    /// squadId에 해당하는 몬스터들 배치
    /// </summary>
    public void PlaceMonster(int squadId)
    {
        Managers.DataMng.MonsterSquadDataDict.TryGetValue(squadId, out MonsterSquadData squadData);
        if (squadData == null)
        {
            Debug.LogError($"Failed to get MonsterSquadData, squadId: {squadId}");
            return;
        }

        foreach (MonsterSquad_MonsterData monsterData in squadData.Monsters)
        {
            Monster monster = Managers.ObjectMng.SpawnMonster(monsterData.DataId);
            MonsterGrid[monsterData.y, monsterData.x].PlaceCreature(monster);
            monster.LookFront();
            _battleManager.AliveMonsters.Add(monster);
            _battleManager.Monsters.Add(monster);
        }
    }

    /// <summary>
    /// targetCell로 Creature 위치 변경.
    /// </summary>
    public void MoveCreature(Creature creature, BattleGridCell targetCell)
    {
        // Creature끼리의 위치 교환을 위해 밑의 if문 필요.
        // 조건이 없을 시, A를 B의 위치로 옮긴 후 B를 A의 위치로 옮길 때 문제가 생김.
        if(creature.StandingCell.PlacedCreature == creature) creature.StandingCell.PlacedCreature = null;
        targetCell.PlaceCreature(creature);

        if(_battleManager.BattleState == BattleState.HeroPlacement && creature is Hero hero) Managers.HeroMng.HeroParty.SaveBattlePosition(hero.InstanceId, new Vector2Int(targetCell.Column, targetCell.Row));
    }

    /// <summary>
    /// 두 Creature의 위치 교환.
    /// </summary>
    public void SwapCreaturePosition(Creature creature1, Creature creature2)
    {
        var tempCell = creature1.StandingCell;
        MoveCreature(creature1, creature2.StandingCell);
        MoveCreature(creature2, tempCell);
    }

    /// <summary>
    /// action의 타겟 가능한 셀 하이라이트 효과.
    /// </summary>
    public void HighlightTargettableCells(BattleAction action)
    {
        var targetables = action.TargetSelector.TargetableCells;

        foreach (var targetable in targetables)
        {
            targetable.HighlightOutline();
        }
    }

    /// <summary>
    /// side에 해당하는 그리드 배열 반환.
    /// </summary>
    public BattleGridCell[,] SideToGrid(GridSide side)
    {
        return side == GridSide.HeroSide ? HeroGrid : MonsterGrid;
    }

    /// <summary>
    /// 모든 그리드 셀의 색 초기화.
    /// </summary>
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

    public void Clear()
    {
        ResetAllCellColor();
        for (int row = 0; row < GlobalValues.BATTLEGRID_ROW_COUNT; row++)
        {
            for (int col = 0; col < GlobalValues.BATTLEGRID_COL_COUNT; col++)
            {
                HeroGrid[row, col] = null;
                MonsterGrid[row, col] = null;
            }
        }

        HeroGrid = null;
        MonsterGrid = null;
    }
}
