using System.Collections;


public class Bag : BattleSkill
{
    public override BattleActionEffectRange EffectRange { get; protected set; } = new DummyRange();
    public override BattleActionTargetSelector TargetSelector { get; protected set; } = new DummySelector();

    public override IEnumerator Execute()
    {
        Managers.BattleMng.UI.BattleBagUI.ShowInstantly();
        yield return null;
    }
}
