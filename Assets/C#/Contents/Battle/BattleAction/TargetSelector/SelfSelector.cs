/// <summary>
/// 자기 자신을 선택하는 BattleActionTargetSelector
/// </summary>
public class SelfSelector : BattleActionTargetSelector
{
    public override bool NeedTargetSelection { get; protected set; } = false;
    public override void CalculateTargetableCells()
    {
        TargetableCells.Clear();
        var currentcell = Managers.BattleMng.CurrentTurnCreature.StandingCell;
        TargetableCells.Add(currentcell);
    }
}