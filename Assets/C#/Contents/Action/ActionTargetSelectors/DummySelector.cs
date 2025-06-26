

/// <summary>
/// 아무것도 선택하지 않는 더미 셀렉터
/// </summary>
public class DummySelector : ActionTargetSelector
{
    public override bool NeedTargetSelection { get; protected set; } = false;
    public override void CalculateTargetableCells() { }
}
