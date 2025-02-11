using UnityEngine;

public class BattleMouseInputHandler
{
    private Camera _camera;
    private BattleGridSystem _battleGridSystem;
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
                OnMouseOverCell();
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
                OnMouseOverCell();
                break;
        }
    }

    private void OnMouseOverCell()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: 100f, layerMask: GlobalValues.LAYERMASK_BATTLEGRIDCELL))
        {
            _battleGridSystem.CurrentMouseOverCell = rayHit.transform.gameObject.GetComponent<BattleGridCell>();
        }
        else
        {
            _battleGridSystem.CurrentMouseOverCell = null;
        }
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
        if (_battleGridSystem.CurrentMouseOverCell?.PlacedCreature == null) return;

        _draggingCreature = _battleGridSystem.CurrentMouseOverCell.PlacedCreature;
        _dragStartCell = _battleGridSystem.CurrentMouseOverCell;
    }

    private void OnDragging()
    {
        if (_draggingCreature == null) return;
        _draggingCreature.transform.position = GetMouseWorldPosition();
    }

    private void OnDragEnd()
    {
        if (_draggingCreature == null) return;
        if (_battleGridSystem.CurrentMouseOverCell != null && _battleGridSystem.CurrentMouseOverCell.GridSide == GridSide.HeroSide)
        {
            if (_battleGridSystem.CurrentMouseOverCell.PlacedCreature == null)
            {
                _battleGridSystem.MoveCreature(_draggingCreature, _battleGridSystem.CurrentMouseOverCell);
            }
            else
            {
                _battleGridSystem.MoveCreature(_battleGridSystem.CurrentMouseOverCell.PlacedCreature, _dragStartCell);
                _battleGridSystem.MoveCreature(_draggingCreature, _battleGridSystem.CurrentMouseOverCell);
            }
        }
        else
        {
            _battleGridSystem.MoveCreature(_draggingCreature, _dragStartCell);
        }
        _draggingCreature = null;
        _dragStartCell = null;
    }

    private void OnClickGridCell()
    {
        if (_battleGridSystem.CurrentMouseOverCell == null || Managers.BattleMng.BattleState != BattleState.ActionTargetSelecting)
            return;

        //CurrentAction.Equip(this);
        //TargetCell = CurrentMouseOverCell;

        //if (!CurrentAction.CanStartAction())
        //{
        //    CurrentAction.UnEquip();
        //    TargetCell = null;
        //    return;
        //}

        //CreatureBattleState = CreatureBattleState.ActionProceed;

        _battleGridSystem.CurrentMouseOverCell.RevertOutlineColor();
    }
}
