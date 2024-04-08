using DG.Tweening;
using UnityEngine;

public abstract class JumpAttackAction : BaseAction
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
        Animator.Play("JumpFWD");
    }

    public override void OnHandleAction()
    {
        if (TargetCell.CellCreature == null)
            return;
        
        Creature targetCreature = TargetCell.CellCreature;
        targetCreature.OnDamage(Owner.CreatureStat.Attack * CoinHeadNum / CoinNum, 1);
    }
    
    public override void OnJumpFWDStart()
    {
        Owner.transform.DOMove(TargetCell.transform.position + _meleeAttackRange, 0.433f).OnComplete(OnJumpFWDEnd);
    }
    
    public override void OnJumpFWDEnd()
    {
        Animator.Play("Attack1");
    }

    public override void OnAttackEnd()
    {
        Animator.Play("JumpBWD");
    }
    
    public override void OnJumpBWDStart()
    {
        Owner.transform.DOMove(Owner.Cell.transform.position, 0.433f).OnComplete(OnActionEnd);
    }
    
}