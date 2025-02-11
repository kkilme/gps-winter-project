using DG.Tweening;
using UnityEngine;

public abstract class BattleGridCell : MonoBehaviour
{
    public Creature PlacedCreature { get; set; }
    public GridSide GridSide { get; protected set; }
    public int Row { get; protected set; }
    public int Col { get; protected set; }

    private SpriteRenderer _outline;
    private SpriteRenderer _fill;
    private Color _outlineOriginalColor;
    private Color _fillOriginalColor;
    private Tweener _outlineColorTween;
    private Tweener _fillColorTween;

    private void Start()
    {
        _outline = GetComponent<SpriteRenderer>();
        _fill = GetComponentInChildren<SpriteRenderer>();
        _outlineOriginalColor = _outline.color;
        _fillOriginalColor = _fill.color;
    }

    public void Init(int row, int col, GridSide gridSide)
    {
        Row = row;
        Col = col;
        GridSide = gridSide;
    }

    public void PlaceCreature(Creature creature)
    {
        PlacedCreature = creature;
        creature.CurrentCell = this;
        creature.gameObject.transform.position = transform.position;
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

    //기본 Unity 메소드 OnMouseEnterEvent & OnMouseExit 사용 시, Hero 및 Enemy 오브젝트에 의해 MouseOver가 가로막힘
    public abstract void HighlightOutline();
    public abstract void HighlightFill();

}
