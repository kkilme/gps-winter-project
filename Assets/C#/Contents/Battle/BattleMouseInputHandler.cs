using UnityEngine;

public class BattleMouseInputHandler
{
    public BattleGridCell CurrentMouseOverCell { get; private set; }
    private BattleGridSystem _battleGridSystem;
    private BaseAction _currentAction => Managers.BattleMng.CurrentAction;

    private Camera _camera;

    private Creature _draggingCreature;
    private BattleGridCell _dragStartCell;

    public void Init()
    {
        _camera = Camera.main;
        _battleGridSystem = Managers.BattleMng.BattleGridSystem;
    }

    public void HandleMouseOnPlacementPhase(MouseEvent mouseEvent)
    {
        switch (mouseEvent)
        {
            case MouseEvent.Hover:
                OnMouseHover_Default();
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
                OnMouseHover_Default();
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

    private void OnMouseHover_Default()
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
        if (CurrentMouseOverCell == null || Managers.BattleMng.BattleState != BattleState.ActionTargetSelecting)
            return;

        //CurrentAction.Equip(this);
        //TargetCell = CurrentMouseOverCell;

        //if (!CurrentAction.IsExecutable())
        //{
        //    CurrentAction.UnEquip();
        //    TargetCell = null;
        //    return;
        //}

        //CreatureBattleState = CreatureBattleState.ActionProceed;

        CurrentMouseOverCell.RevertOutlineColor();
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
        if (CurrentMouseOverCell?.PlacedCreature == null) return;

        _draggingCreature = CurrentMouseOverCell.PlacedCreature;
        _dragStartCell =    CurrentMouseOverCell;
    }

    private void OnDragging()
    {
        if (_draggingCreature == null) return;
        _draggingCreature.transform.position = GetMouseWorldPosition();
    }

    private void OnDragEnd()
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
                _battleGridSystem.MoveCreature(CurrentMouseOverCell.PlacedCreature, _dragStartCell);
                _battleGridSystem.MoveCreature(_draggingCreature, CurrentMouseOverCell);
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
