using UnityEngine;

public class BattleGridSystem
{
    public BattleGridCell[,] HeroGrid { get; protected set; } = new BattleGridCell[2, 3];
    public BattleGridCell[,] EnemyGrid { get; protected set; } = new BattleGridCell[2, 3];

    private BattleGridCell _currentMouseOverCell;
    public BattleGridCell CurrentMouseOverCell
    {
        get => _currentMouseOverCell;
        protected set
        {
            if (_currentMouseOverCell == value)
                return;

            if (_currentMouseOverCell != null) _currentMouseOverCell.OnMouseOut();
            _currentMouseOverCell = value;
            if (_currentMouseOverCell != null) _currentMouseOverCell.OnMouseIn();
        }
    }

    private Camera _camera;
    private BattleManager _battleManager => Managers.BattleMng;
    private Creature _draggingCreature;
    private BattleGridCell _dragStartCell;

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
                EnemyGrid[row, col] = Util.FindChild<BattleGridCell>(enemyGrid, $"BattleGridCell ({row}, {col})");
                EnemyGrid[row, col].Init(row, col, Define.GridSide.EnemySide);
            }
        }

        _camera = Camera.main;
    }

    public void PlaceHero()
    {
        foreach (Hero hero in Managers.ObjectMng.HeroParty.Heroes)
        {
            Vector2Int pos = Managers.ObjectMng.HeroParty.BattlePositions[hero];
            HeroGrid[pos.y, pos.x].PlaceCreature(hero);
            hero.transform.LookAt(EnemyGrid[pos.y, 2 - pos.x].transform.position);
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
            EnemyGrid[pos.y, pos.x].PlaceCreature(monster);
            monster.transform.LookAt(HeroGrid[pos.y, 2 - pos.x].transform.position);
            _battleManager.Monsters.Add(monster);
        }
    }

    public void MoveCreature(Creature creature, BattleGridCell targetCell)
    {
        if(creature.Cell.PlacedCreature == creature) creature.Cell.PlacedCreature = null;
        targetCell.PlaceCreature(creature);
        //creature.transform.LookAt(HeroGrid[targetCell.Row, 2 - targetCell.Col].transform.position);
    }

    public void HandleMouseInputOnBattlePhase(Define.MouseEvent mouseEvent)
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

    public void HandleMouseInputOnPlacementPhase(Define.MouseEvent mouseEvent) 
    {
        switch (mouseEvent)
        {
            case Define.MouseEvent.Hover:
                OnMouseOverCell();
                break;
            case Define.MouseEvent.PointerDown:
                OnDragStart();
                break;
            case Define.MouseEvent.Press:
                OnDragging();
                break;
            case Define.MouseEvent.PointerUp:
                OnDragEnd();
                break;
        }
    }

    private void OnDragStart()
    {
        if (_battleManager.BattleState != Define.BattleState.HeroPlacement || CurrentMouseOverCell?.PlacedCreature == null) return;

        _draggingCreature = CurrentMouseOverCell.PlacedCreature;
        _dragStartCell = CurrentMouseOverCell;
    }

    private void OnDragging()
    {
        if (_draggingCreature == null) return;
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        _draggingCreature.transform.position = mouseWorldPos;
    }

    private void OnDragEnd()
    {
        if (_draggingCreature == null) return;
        if (CurrentMouseOverCell != null && CurrentMouseOverCell.GridSide == Define.GridSide.HeroSide)
        {
            if (CurrentMouseOverCell.PlacedCreature == null)
            {   
                MoveCreature(_draggingCreature, CurrentMouseOverCell);
            }
            else
            {
                Creature creature = CurrentMouseOverCell.PlacedCreature;
                MoveCreature(creature, _dragStartCell);
                MoveCreature(_draggingCreature, CurrentMouseOverCell);
            }
        }
        else
        {
            MoveCreature(_draggingCreature, _dragStartCell);
        }
        _draggingCreature = null;
        _dragStartCell = null;
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
        if (CurrentMouseOverCell == null || _battleManager.BattleState != Define.BattleState.ActionTargetSelecting)
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

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: 100f, layerMask: LayerMask.GetMask("BattleGround")))
        {
            return rayHit.point;
        }
        return new Vector3(Define.BATTLEFIELD_POS_X, 0f, Define.BATTLEFIELD_POS_Z);
    }
}
