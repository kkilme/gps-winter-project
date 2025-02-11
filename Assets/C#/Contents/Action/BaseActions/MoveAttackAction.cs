using DG.Tweening;
using UnityEngine;

public abstract class MoveAttackAction : BaseAction
{
    protected Vector3 _meleeAttackRange;

    public override bool CanStartAction()
    {
        if (Owner.CreatureType == CreatureType.Hero && TargetCell.GridSide == GridSide.HeroSide)
            return false;
        if (Owner.CreatureType == CreatureType.Monster && TargetCell.GridSide == GridSide.EnemySide)
            return false;

        return true;
    }
    
    public override void OnStartAction()
    {
        Animator.Play("Move");
        OnMoveStart();
    }
    
    public override void OnHandleAction()
    {
        if (TargetCell.PlacedCreature == null)
            return;
        
        Creature targetCreature = TargetCell.PlacedCreature;
        targetCreature.OnDamage(Owner.CreatureStat.Attack * (CoinHeadNum / CoinNum), 1);
    }
    
    public override void OnMoveStart()
    {
        Owner.transform.DOMove(TargetCell.transform.position + _meleeAttackRange, 0.8f).OnComplete(OnMoveFWDEnd);
    }
    
    public override void OnMoveFWDEnd()
    {
        Animator.Play("Attack1");
    }

    public override void OnAttackEnd()
    {
        Animator.Play("MoveBWD");
        OnMoveBWDStart();
    }
    
    public override void OnMoveBWDStart()
    {
        Owner.transform.DOMove(Owner.CurrentCell.transform.position, 0.8f).OnComplete(OnActionEnd);
    }
}