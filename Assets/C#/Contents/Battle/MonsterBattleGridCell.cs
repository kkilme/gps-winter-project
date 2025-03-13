using UnityEngine;
public class MonsterBattleGridCell : BattleGridCell
{
    public override void HighlightOutline()
    {
        ChangeOutlineColor(GlobalValues.ENEMYGRID_OUTLINE_HIGHLIGHT_COLOR);
    }

    public override void HighlightFill()
    {
        ChangeFillColor(GlobalValues.ENEMYGRID_FILL_HIGHLIGHT_COLOR);
    }

}
