using DG.Tweening;
using System.Collections;
using UnityEngine;

public class HealPotionAction : ItemAction
{
    public override BattleActionEffectRange EffectRange { get; protected set; } = new SingleRange();
    public override BattleActionTargetSelector TargetSelector { get; protected set; } = new SelfSelector();

    public HealPotionAction(Item item) : base(item) { }

    public override IEnumerator Execute()
    {
        SelectedTargetCell.PlacedCreature.TakeHeal(.2f);
        Managers.AreaMng.Items.Remove(Item); // 아이템 사용 후 인벤토리에서 제거
        Managers.BattleMng.OnActionEnd();
        yield return null;
    }
}