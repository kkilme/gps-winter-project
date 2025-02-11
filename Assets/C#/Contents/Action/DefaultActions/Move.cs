using DG.Tweening;

public class Move : BaseAction
{
    public override void SetInfo(int templateId)
    {
        base.SetInfo(templateId);
        
        ActionTargetType = ActionTargetType.Single;
        TargetSelector = new MoveTargetSelector();
    }
    
    public override bool CanStartAction()
    {
        if (Owner.CreatureType == CreatureType.Hero && TargetCell.GridSide == GridSide.MonsterSide)
            return false;
        if (Owner.CreatureType == CreatureType.Monster && TargetCell.GridSide == GridSide.HeroSide)
            return false;

        if (TargetCell.PlacedCreature != null)
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
        
        OnActionEnd();
    }
}