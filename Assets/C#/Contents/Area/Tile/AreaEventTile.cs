using DG.Tweening;
using UnityEngine;
using System.Collections;

public enum TileColorChangeType
{
    Highlight,
    Reset,
    ToNormal
}

// 플레이 가능 영역에 생성되는, 플레이어가 이동 가능한 타일.
// Note: 플레이어가 밟고 서있는 육각형 블록은 AreaBaseTile
public abstract class AreaEventTile: MonoBehaviour
{
    public Define.AreaTileType TileType;
    [SerializeField]
    private SpriteRenderer _outline;  // 셀 모서리 스프라이트
    [SerializeField]
    private SpriteRenderer _fill;     // 셀 내부 스프라이트
    [SerializeField]
    private GameObject _icon;           // 아이콘 오브젝트

    private Color _outlineColor;      // 모서리 스프라이트 색
    private Color _fillColor;         // 내부 스프라이트 색
    [SerializeField]
    private Color _outlineHighlightColor; // 플레이어의 이동 가능 지점을 보여줄 때 하이라이트되어 변하는 indicator 색
    [SerializeField]
    private Color _fillHighlightColor; // 플레이어의 이동 가능 지점을 보여줄 때 하이라이트되어 변하는 fill 색

    private Tweener _outlineColorTween; // DoTween을 통해 TileObject의 스프라이트 색을 바꾸는데, Tweener는 이 작업을 의미함. 작업 도중에 취소 시 사용
    private Tweener _fillColorTween;

    public void Init()
    {   
        _outlineColor = _outline.color;
        _fillColor = _fill.color;
        InitMesh();
    }

    public void ChangeColor(TileColorChangeType changeType, float duration = 0.3f)
    {
        KillColorTween();
        switch (changeType)
        {
            case TileColorChangeType.Highlight:
                _outlineColorTween = _outline.DOColor(_outlineHighlightColor, duration).OnComplete(() => { _outlineColorTween = null; });
                _fillColorTween = _fill.DOColor(_fillHighlightColor, duration).OnComplete(() => { _fillColorTween = null; });
                break;
            case TileColorChangeType.Reset:
                _outlineColorTween = _outline.DOColor(_outlineColor, duration).OnComplete(() => { _outlineColorTween = null; });
                _fillColorTween = _fill.DOColor(_fillColor, duration).OnComplete(() => { _fillColorTween = null; });
                break;
        }

    }

    // 기존 진행중인 colorTween을 중지, 삭제
    private void KillColorTween()
    {
        _outlineColorTween?.Kill();
        _outlineColorTween = null;

        _fillColorTween?.Kill();
        _fillColorTween = null;
    }

    // Sprite로 Mesh를 만들고 Collider에 적용: raycast를 위해 필요
    private void InitMesh()
    {
        Mesh mesh = Util.SpriteToMesh(_fill.sprite);
        gameObject.transform.GetComponentInChildren<MeshCollider>().sharedMesh = mesh;
    }

    public void Destroy()
    {
        if (_icon != null)
            _icon.GetComponent<SpriteRenderer>().DOFade(0, 0.5f).OnComplete(() => { GameObject.Destroy(_icon); });
    }

    public abstract void OnTileEnter();

    public abstract void OnTileEventFinish();
}
