
public class HeroBattleGridCell : BattleGridCell
{
    public override void HighlightOutline()
    {
        ChangeOutlineColor(GlobalValues.HEROGRID_OUTLINE_HIGHLIGHT_COLOR);
    }

    public override void HighlightFill()
    {
        ChangeFillColor(GlobalValues.HEROGRID_FILL_HIGHLIGHT_COLOR);
    }

}
