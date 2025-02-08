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
                HeroGrid[row, col].Init(row, col, GlobalEnums.GridSide.HeroSide);
                EnemyGrid[row, col] = Util.FindChild<BattleGridCell>(enemyGrid, $"BattleGridCell ({row}, {col})");
                EnemyGrid[row, col].Init(row, col, GlobalEnums.GridSide.EnemySide);
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
        // 셀에서 Creature 배치 해제. 서로의 위치 교환을 위해 if문 조건 필요.
        // 조건이 없을 시, A를 B의 위치로 옮긴 후 B를 A의 위치로 옮길 때 문제가 생김.
        if(creature.CurrentCell.PlacedCreature == creature) creature.CurrentCell.PlacedCreature = null;
        targetCell.PlaceCreature(creature);
        //creature.transform.LookAt(HeroGrid[targetCell.Row, 2 - targetCell.Col].transform.position);
    }

    public void HandleMouseInputOnPlacementPhase(GlobalEnums.MouseEvent mouseEvent) 
    {
        switch (mouseEvent)
        {
            case GlobalEnums.MouseEvent.Hover:
                OnMouseOverCell();
                break;
            case GlobalEnums.MouseEvent.PointerDown:
                OnDragStart();
                break;
            case GlobalEnums.MouseEvent.Press:
                OnDragging();
                break;
            case GlobalEnums.MouseEvent.PointerUp:
                OnDragEnd();
                break;
        }
    }
    public void HandleMouseInputOnBattlePhase(GlobalEnums.MouseEvent mouseEvent)
    {   
        switch (mouseEvent)
        {
            case GlobalEnums.MouseEvent.Hover:
                OnMouseOverCell();
                break;
            case GlobalEnums.MouseEvent.PointerDown:
                OnClickGridCell();
                break;
        }
    }

    // Drag 관련은 Hero 배치 단계에서만 사용됨
    private void OnDragStart()
    {
        if (CurrentMouseOverCell?.PlacedCreature == null) return;

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
        if (CurrentMouseOverCell != null && CurrentMouseOverCell.GridSide == GlobalEnums.GridSide.HeroSide)
        {
            if (CurrentMouseOverCell.PlacedCreature == null)
            {   
                // 빈 셀일 시 이동
                MoveCreature(_draggingCreature, CurrentMouseOverCell);
            }
            else
            {   
                // 셀에 다른 Creature가 존재하면 위치를 교환
                MoveCreature(CurrentMouseOverCell.PlacedCreature, _dragStartCell);
                MoveCreature(_draggingCreature, CurrentMouseOverCell);
            }
        }
        else
        {   
            // 예외 시 기존 위치로 복귀
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
        if (CurrentMouseOverCell == null || _battleManager.BattleState != GlobalEnums.BattleState.ActionTargetSelecting)
            return;

        //CurrentAction.Equip(this);
        //TargetCell = CurrentMouseOverCell;

        //if (!CurrentAction.CanStartAction())
        //{
        //    CurrentAction.UnEquip();
        //    TargetCell = null;
        //    return;
        //}

        //CreatureBattleState = GlobalEnums.CreatureBattleState.ActionProceed;

        CurrentMouseOverCell.RevertColor();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: 100f, layerMask: LayerMask.GetMask("BattleGround")))
        {
            return rayHit.point;
        }
        return new Vector3(GlobalValues.BATTLEFIELD_POS_X, 0f, GlobalValues.BATTLEFIELD_POS_Z);
    }
}
