using DG.Tweening;
using UnityEngine;

public class BattleGridCell : MonoBehaviour
{
    public Creature PlacedCreature { get; set; }
    public GlobalEnums.GridSide GridSide { get; protected set; }
    public int Row { get; protected set; }
    public int Col { get; protected set; }

    private SpriteRenderer _indicator;
    private Color _originalColor;

    private Tweener _colorTween;

    private void Start()
    {
        _indicator = GetComponent<SpriteRenderer>();
        _originalColor = _indicator.color;
        transform.position += new Vector3(0f, 0.03f, 0f);
    }

    public void Init(int row, int col, GlobalEnums.GridSide gridSide)
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

    private void ChangeColor(Color color, float duration = 0.3f)
    {
        KillColorTween();
        _colorTween = _indicator.DOColor(color, duration).OnComplete(() => { _colorTween = null; });
    }

    public void RevertColor()
    {
        ChangeColor(_originalColor);
    }

    // 진행중인 colorTween을 중지, 삭제
    private void KillColorTween()
    {
        _colorTween?.Kill();
        _colorTween = null;
    }

    //기본 Unity 메소드 OnMouseEnterEvent & OnMouseExit 사용 시, Hero 및 Enemy 오브젝트에 의해 MouseOver가 가로막힘
    public void Highlight()
    {
        if (GridSide == GlobalEnums.GridSide.HeroSide)
        {
            ChangeColor(GlobalValues.HEROGRID_HIGHLIGHT_COLOR);
        }
        else if (GridSide == GlobalEnums.GridSide.EnemySide)
        {
            ChangeColor(GlobalValues.ENEMYGRID_HIGHLIGHT_COLOR);
        }
    }
    public void HighlightHarder()
    {
        if (GridSide == GlobalEnums.GridSide.HeroSide)
        {
            ChangeColor(GlobalValues.HEROGRID_HARD_HIGHLIGHT_COLOR);
        }
        else if (GridSide == GlobalEnums.GridSide.EnemySide)
        {
            ChangeColor(GlobalValues.ENEMYGRID_HARD_HIGHLIGHT_COLOR);
        }
    }
}
