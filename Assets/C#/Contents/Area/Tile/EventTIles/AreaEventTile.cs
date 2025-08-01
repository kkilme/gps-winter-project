using DG.Tweening;
using UnityEngine;
using System.Collections;

public enum TileColorChangeType
{
    Highlight,
    Reset,
    ToNormal
}

// 플레이 가능 영역에 생성되는, 플레이어가 이동 가능한 이벤트 타일.
// Note: 플레이어가 밟고 서있는 육각형 블록 게임오브젝트는 AreaBaseTile
public abstract class AreaEventTile: MonoBehaviour           
{
    public AreaTileType TileType;

    protected SpriteRenderer _outline;  // 셀 모서리 스프라이트
    protected SpriteRenderer _fill;     // 셀 내부 스프라이트
    protected GameObject _icon;           // 아이콘 오브젝트

    [SerializeField]
    protected EventTileColorData _colorData; // 타일 색상 데이터

    protected Tweener _outlineColorTween; // DoTween을 통해 TileObject의 스프라이트 색을 바꾸는데, Tweener는 이 작업을 의미함. 작업 도중에 취소 시 사용.
    protected Tweener _fillColorTween;

    protected bool _willBeDestroyed = false; // 타일이 파괴될 예정인지 여부

    public void Init()
    {   
        InitSprite();
        InitMesh();
    }

    private void InitSprite()
    {
        _fill = transform.Find("fill")?.GetComponent<SpriteRenderer>();
        _fill.color = _colorData.FillDefaultColor;
        _outline = transform.Find("outline")?.GetComponent<SpriteRenderer>();
        _outline.color = _colorData.OutlineDefaultColor;
        _icon = transform.Find("icon")?.gameObject;
    }

    /// <summary>
    /// Sprite로 Mesh를 만들고 Collider에 적용: raycast를 위해 필요
    /// </summary>
    private void InitMesh()
    {
        Mesh mesh = RenderUtility.SpriteToMesh(_fill.sprite);
        gameObject.transform.GetComponentInChildren<MeshCollider>().sharedMesh = mesh;
    }

    public void ChangeColor(TileColorChangeType changeType, float duration = 0.3f)
    {
        if(_willBeDestroyed) return; // 타일이 파괴될 예정이라면 색을 바꾸지 않음

        KillColorTween();
        switch (changeType)
        {
            case TileColorChangeType.Highlight:
                _fillColorTween = _fill.DOColor(_colorData.FillHighlightColor, duration).OnComplete(() => { _fillColorTween = null; });
                _outlineColorTween = _outline.DOColor(_colorData.OutlineHighlightColor, duration).OnComplete(() => { _outlineColorTween = null; });
                break;
            case TileColorChangeType.Reset:
                _fillColorTween = _fill.DOColor(_colorData.FillDefaultColor, duration).OnComplete(() => { _fillColorTween = null; });
                _outlineColorTween = _outline.DOColor(_colorData.OutlineDefaultColor, duration).OnComplete(() => { _outlineColorTween = null; });
                break;
        }

    }

    /// <summary>
    /// 기존 진행중인 colorTween을 중지, 삭제
    /// </summary>
    private void KillColorTween()
    {
        _outlineColorTween?.Kill();
        _outlineColorTween = null;

        _fillColorTween?.Kill();
        _fillColorTween = null;
    }


    public void Destroy()
    {
        if (_willBeDestroyed) return;

        _willBeDestroyed = true;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(_fill.DOFade(0, 0.5f));
        sequence.Join(_outline.DOFade(0, 0.5f));
        if (_icon != null)
        {
            sequence.Join(_icon.GetComponent<SpriteRenderer>().DOFade(0, 0.5f));
        }

        sequence.Play().OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    public abstract void OnTileEnter();
}
