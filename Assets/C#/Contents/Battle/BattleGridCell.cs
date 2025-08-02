using DG.Tweening;
using UnityEngine;

public abstract class BattleGridCell : MonoBehaviour
{
    public Creature PlacedCreature { get; set; }
    public GridSide GridSide { get; protected set; }
    public int Row { get; protected set; }
    public int Column { get; protected set; }

    private SpriteRenderer _outline;
    private SpriteRenderer _fill;
    private Color _outlineOriginalColor;
    private Color _fillOriginalColor;
    private Tweener _outlineColorTween;
    private Tweener _fillColorTween;

    private void Awake()
    {
        _outline = GetComponent<SpriteRenderer>();
        _fill = GlobalUtility.FindChild<SpriteRenderer>(gameObject, "fill");
        _outlineOriginalColor = _outline.color;
        _fillOriginalColor = _fill.color;
    }

    public void Init(int row, int col, GridSide gridSide)
    {
        Row = row;
        Column = col;
        GridSide = gridSide;
    }

    public void PlaceCreature(Creature creature)
    {
        PlacedCreature = creature;
        creature.StandingCell = this;
        creature.gameObject.transform.position = transform.position;
    }

    public void RemoveCreature()
    {
        if (PlacedCreature != null)
            PlacedCreature.StandingCell = null;
        PlacedCreature = null;
    }

    public bool IsEmpty()
    {
        return PlacedCreature == null;
    }

    protected void ChangeOutlineColor(Color color, float duration = 0.3f)
    {
        _outlineColorTween?.Kill();
        _outlineColorTween = _outline.DOColor(color, duration).OnComplete(() => { _outlineColorTween = null; });
    }

    public void RevertOutlineColor()
    {
        ChangeOutlineColor(_outlineOriginalColor);
    }

    protected void ChangeFillColor(Color color, float duration = 0.3f)
    {
        _fillColorTween?.Kill();
        _fillColorTween = _fill.DOColor(color, duration).OnComplete(() => { _fillColorTween = null; });
    }

    public void RevertFillColor()
    {
        ChangeFillColor(_fillOriginalColor);
    }

    public void HighlightAll()
    {
        HighlightOutline();
        HighlightFill();
    }

    public abstract void HighlightOutline();
    public abstract void HighlightFill();

}
