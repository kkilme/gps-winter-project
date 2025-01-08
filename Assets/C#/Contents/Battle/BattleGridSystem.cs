using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGridSystem
{
    public BattleGridCell[,] HeroGrid { get; protected set; } = new BattleGridCell[2, 3];
    public BattleGridCell[,] MonsterGrid { get; protected set; } = new BattleGridCell[2, 3];

    private BattleGridCell _currentMouseOverCell;
    public BattleGridCell CurrentMouseOverCell
    {
        get => _currentMouseOverCell;
        protected set
        {
            if (_currentMouseOverCell == value)
                return;

            if (_currentMouseOverCell != null) _currentMouseOverCell.OnMouseExited();
            _currentMouseOverCell = value;
            if (_currentMouseOverCell != null) _currentMouseOverCell.OnMouseEntered();
        }
    }

    private Camera _camera;

    public void Init()
    {
        GameObject heroGrid = GameObject.FindGameObjectWithTag("HeroGrid");
        GameObject enemyGrid = GameObject.FindGameObjectWithTag("EnemyGrid");

        for (int row = 0; row < 2; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                HeroGrid[row, col] = Util.FindChild<BattleGridCell>(heroGrid, $"BattleGridCell ({row}, {col})");
                HeroGrid[row, col].Init(row, col, Define.GridSide.HeroSide);
                MonsterGrid[row, col] = Util.FindChild<BattleGridCell>(enemyGrid, $"BattleGridCell ({row}, {col})");
                MonsterGrid[row, col].Init(row, col, Define.GridSide.EnemySide);
            }
        }

        Managers.InputMng.MouseAction -= HandleMouseInput;
        Managers.InputMng.MouseAction += HandleMouseInput;

        _camera = Camera.main;
    }

    public void PlaceHero()
    {
        foreach (Hero hero in Managers.ObjectMng.HeroParty.Heroes)
        {
            Vector2Int pos = Managers.ObjectMng.HeroParty.BattlePositions[hero];
            HeroGrid[pos.y, pos.x].PlaceCreature(hero);
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

        foreach(MonsterSquad_MonsterData monsterData in squadData.Monsters)
        {
            Monster monster = Managers.ObjectMng.SpawnMonster(monsterData.DataId);
            Vector2Int pos = new Vector2Int(monsterData.x, monsterData.y);
            MonsterGrid[pos.y, pos.x].PlaceCreature(monster);
        }
    }

    private void HandleMouseInput(Define.MouseEvent mouseEvent)
    {
        switch (mouseEvent)
        {
            case Define.MouseEvent.Hover:
                OnMouseOverCell();
                break;
            case Define.MouseEvent.PointerDown:
                OnClickGridCell();
                break;
        }
        
    }

    private void OnMouseOverCell()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: 100f, layerMask: LayerMask.GetMask("BattleGridCell")))
        {
            CurrentMouseOverCell = rayHit.transform.gameObject.GetComponent<BattleGridCell>();
        }
        else CurrentMouseOverCell = null;
    }

    private void OnClickGridCell()
    {
        if (CurrentMouseOverCell == null)
            return;

        //CurrentAction.Equip(this);
        //TargetCell = CurrentMouseOverCell;

        //if (!CurrentAction.CanStartAction())
        //{
        //    CurrentAction.UnEquip();
        //    TargetCell = null;
        //    return;
        //}

        //CreatureBattleState = Define.CreatureBattleState.ActionProceed;

        CurrentMouseOverCell.RevertColor();
    }
}
