using DG.Tweening;

public class MoveAction : BaseAction
{
    public override void SetInfo(int templateId)
    {
        base.SetInfo(templateId);
        
        ActionTargetType = Define.ActionTargetType.Single;
    }
    
    public override bool CanStartAction()
    {
        if (Owner.CreatureType == Define.CreatureType.Hero && TargetCell.GridSide == Define.GridSide.MonsterSide)
            return false;
        if (Owner.CreatureType == Define.CreatureType.Monster && TargetCell.GridSide == Define.GridSide.HeroSide)
            return false;

        if (TargetCell.CellCreature != null)
            return false;
        
        return true;
    }
    
    public override void OnStartAction()
    {
        OnMoveStart();
    }
    
    public override void OnMoveStart()
    {
        Animator.Play("Move");
        
        Owner.transform.DOMove(TargetCell.transform.position, 0.8f).OnComplete(OnHandleAction);
    }
    
    public override void OnHandleAction()
    {
        Managers.BattleMng.MoveCreature(Owner, TargetCell, false);
        
        OnActionEnd();
    }
}