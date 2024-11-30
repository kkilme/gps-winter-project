using DG.Tweening;
using UnityEngine;

public abstract class MoveAttackAction : BaseAction
{
    protected Vector3 _meleeAttackRange;

    public override bool CanStartAction()
    {
        if (Owner.CreatureType == Define.CreatureType.Hero && TargetCell.GridSide == Define.GridSide.HeroSide)
            return false;
        if (Owner.CreatureType == Define.CreatureType.Monster && TargetCell.GridSide == Define.GridSide.MonsterSide)
            return false;

        return true;
    }
    
    public override void OnStartAction()
    {
        Animator.Play("MoveFWD");
        OnMoveStart();
    }
    
    public override void OnHandleAction()
    {
        if (TargetCell.CellCreature == null)
            return;
        
        Creature targetCreature = TargetCell.CellCreature;
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
        Owner.transform.DOMove(Owner.Cell.transform.position, 0.8f).OnComplete(OnActionEnd);
    }
}