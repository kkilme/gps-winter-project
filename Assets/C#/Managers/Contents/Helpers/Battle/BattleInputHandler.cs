using UnityEngine;

// 전투에서 마우스/키보드 입력에 따른 로직 관리
public class BattleInputHandler
{
    public BattleGridCell CurrentMouseOverCell { get; private set; } // 현재 마우스를 올리고 있는 Cell
    private BattleGridSystem _battleGridSystem => Managers.BattleMng.GridSystem;
    private BaseAction _currentAction => Managers.BattleMng.CurrentAction;

    private Camera _camera;

    private Creature _draggingCreature;
    private BattleGridCell _dragStartCell;

    public void Init()
    {
        _camera = Camera.main;
    }

    public void HandleMouseOnPlacementPhase(MouseEvent mouseEvent)
    {
        switch (mouseEvent)
        {
            case MouseEvent.Hover:
                OnMouseHover_PlacementPhase();
                break;
            case MouseEvent.PointerDown:
                OnDragStart();
                break;
            case MouseEvent.Press:
                OnDragging();
                break;
            case MouseEvent.PointerUp:
                OnDragEnd();
                break;
        }
    }

    public void HandleMouseOnBattlePhase(MouseEvent mouseEvent)
    {
        switch (mouseEvent)
        {
            case MouseEvent.Hover:
                OnMouseHover_BattleIdle();
                break;
        }
    }

    public void HandleMouseOnTargetSelect(MouseEvent mouseEvent)
    {
        switch (mouseEvent)
        {
            case MouseEvent.Hover:
                OnMouseHover_TargetSelect();
                break;
            case MouseEvent.PointerDown:
                OnClickGridCell();
                break;
        }
    }

    private void OnMouseHover_PlacementPhase()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: 100f, layerMask: GlobalValues.LAYERMASK_BATTLEGRIDCELL))
        {
            var cell = rayHit.transform.gameObject.GetComponent<BattleGridCell>();
            if (CurrentMouseOverCell == cell)
                return;

            if (CurrentMouseOverCell != null) CurrentMouseOverCell.RevertOutlineColor();
            CurrentMouseOverCell = cell;
            CurrentMouseOverCell.HighlightOutline();
        }
        else
        {
            if (CurrentMouseOverCell != null) CurrentMouseOverCell.RevertOutlineColor();
            CurrentMouseOverCell = null;
        }
    }

    private void OnMouseHover_BattleIdle()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: 100f, layerMask: GlobalValues.LAYERMASK_BATTLEGRIDCELL))
        {
            var cell = rayHit.transform.gameObject.GetComponent<BattleGridCell>();

            if (CurrentMouseOverCell == cell)
                return;

            CurrentMouseOverCell = cell;
        }
        else
        {
            CurrentMouseOverCell = null;
        }
    }

    private void OnMouseHover_TargetSelect()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: 100f, layerMask: GlobalValues.LAYERMASK_BATTLEGRIDCELL))
        {
            var cell = rayHit.transform.gameObject.GetComponent<BattleGridCell>();

            if (CurrentMouseOverCell == cell)
                return;

            if (CurrentMouseOverCell != null) CurrentMouseOverCell.RevertFillColor();
            if (!_currentAction.TargetSelector.IsTargettable(cell))
            {
                CurrentMouseOverCell = null;
                return;
            }
            
            CurrentMouseOverCell = cell;
            CurrentMouseOverCell.HighlightFill();
        }
        else
        {
            if (CurrentMouseOverCell != null) CurrentMouseOverCell.RevertFillColor();
            CurrentMouseOverCell = null;
        }
    }

    private void OnClickGridCell()
    {   
        if (CurrentMouseOverCell == null || !_currentAction.TargetSelector.IsTargettable(CurrentMouseOverCell))
            return;

        Managers.InputMng.RemoveMouseAction(HandleMouseOnTargetSelect);

        _currentAction.SetTarget(CurrentMouseOverCell);
        _battleGridSystem.ResetAllCellColor();
        _currentAction.HighlightAffectedTargets(CurrentMouseOverCell);

        Managers.BattleMng.UI.ChooseTargetUI.Hide();

        // 액션 실행
        CoroutineRunner.Instance.StartCoroutine(_currentAction.Execute());
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: 100f, layerMask: GlobalValues.LAYERMASK_BATTLEGROUND))
        {
            return rayHit.point;
        }
        return new Vector3(GlobalValues.BATTLEFIELD_POS_X, 0f, GlobalValues.BATTLEFIELD_POS_Z);
    }

    private void OnDragStart()
    {
        if (CurrentMouseOverCell?.PlacedCreature == null || CurrentMouseOverCell.GridSide == GridSide.MonsterSide) return;

        _draggingCreature = CurrentMouseOverCell.PlacedCreature;
        _dragStartCell = CurrentMouseOverCell;
    }

    private void OnDragging()
    {
        if (_draggingCreature == null) return;

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        _draggingCreature.transform.position = GetMouseWorldPosition();
    }

    public void OnDragEnd()
    {
        if (_draggingCreature == null) return;
        if (CurrentMouseOverCell != null && CurrentMouseOverCell.GridSide == GridSide.HeroSide)
        {
            if (CurrentMouseOverCell.PlacedCreature == null)
            {
                _battleGridSystem.MoveCreature(_draggingCreature, CurrentMouseOverCell);
            }
            else
            {
                _battleGridSystem.SwapCreaturePosition(_draggingCreature, CurrentMouseOverCell.PlacedCreature);
            }
        }
        else
        {
            _battleGridSystem.MoveCreature(_draggingCreature, _dragStartCell);
        }
        _draggingCreature = null;
        _dragStartCell = null;
    }
}
