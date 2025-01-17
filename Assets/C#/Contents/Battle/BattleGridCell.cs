using DG.Tweening;
using UnityEngine;

public class BattleGridCell : MonoBehaviour
{
    public Creature PlacedCreature { get; set; }
    public Define.GridSide GridSide { get; protected set; }
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

    public void Init(int row, int col, Define.GridSide gridSide)
    {
        Row = row;
        Col = col;
        GridSide = gridSide;
    }

    public void PlaceCreature(Creature creature)
    {
        PlacedCreature = creature;
        creature.Cell = this;
        creature.gameObject.transform.position = transform.position;
    }

    private void ChangeColor(Color color, float duration = 0.3f)
    {
        KillColorTween();
        _colorTween = _indicator.DOColor(color, duration).OnComplete(() => { _colorTween = null; });
    }
    // 진행중인 colorTween을 중지, 삭제
    private void KillColorTween()
    {
        _colorTween?.Kill();
        _colorTween = null;
    }

    public void RevertColor()
    {
        ChangeColor(_originalColor);
    }

    //기본 Unity 메소드 OnMouseEnter & OnMouseExit 사용 시, Hero 및 Enemy 오브젝트에 의해 MouseOver가 가로막힘
    public void OnMouseEntered()
    {
        if (GridSide == Define.GridSide.HeroSide)
        {
            ChangeColor(Color.green);
        }
        else if (GridSide == Define.GridSide.EnemySide)
        {
            ChangeColor(Color.red);
        }
    }

    public void OnMouseExited()
    {
        ChangeColor(_originalColor);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(gameObject.transform.position, transform.right, Color.red);
    }
}
