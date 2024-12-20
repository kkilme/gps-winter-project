using DG.Tweening;
using UnityEngine;
using System.Collections;
using static Define;

public enum TileColorChangeType
{
    Highlight,
    Reset,
    ToNormal
}

// 플레이 가능 영역에 생성되는, 플레이어가 이동 가능한 타일.
// Note: 플레이어가 밟고 서있는 바닥 블록은 AreaBaseTile
public abstract class AreaEventTile
{
    // 색 변화가 가능한 스프라이트를 갖고 있는 셀 오브젝트 (프리팹 이름: grid_hex)
    public GameObject TileObject { get; private set; }
    // 타일의 타입
    public AreaTileType TileType { get; protected set; }
    // 타일의 월드 좌표
    protected Vector3 _worldPosition;
    // 셀 모서리 스프라이트
    protected SpriteRenderer _indicator;
    // 셀 내부 스프라이트
    protected SpriteRenderer _fill;

    // 이 타일이 어떤 타입의 타일인지 나타내는 아이콘 오브젝트
    protected GameObject _icon;
    public GameObject Icon
    {
        get => _icon;
        set
        {
            _icon = value;
            if (value != null)
            {
                _icon.transform.position = TileObject.transform.position;
            }
        }
    }

    protected Color _indicatorColor; // TileObject의 모서리 색상. 타일 타입별로 지정된 색이 다름
    protected Color _fillColor; // TileObject의 내부 색상.
    protected Color _indicatorHighlightColor; // 플레이어의 이동 가능 지점을 보여줄 때 하이라이트되어 변하는 indicator 색상
    protected Color _fillHighlightColor; // 플레이어의 이동 가능 지점을 보여줄 때 하이라이트되어 변하는 fill 색상


    // DoTween을 통해 TileObject의 스프라이트 색을 바꾸는데, 이 작업을 의미함.
    // 이를 통해 작업을 도중에 취소할 수 있음
    private Tweener _indicatorColorTween;
    private Tweener _fillColorTween;

    // isRecycle이 True라면 기존의 TileObject 오브젝트 재활용. 타일의 type만을 바꾸는 데 사용됨 (AreaGrid의 ChangeTile 참조).
    protected AreaEventTile(Vector3 position, GameObject tileObject, bool isRecycle = false)
    {   
        _worldPosition = position;
        TileObject = tileObject;
        InitSprites();
        if (!isRecycle)
        {
            InitMesh();
        }
    }

    public abstract void Init();

    public void ChangeColor(TileColorChangeType changeType, float duration = 0.3f)
    {
        KillColorTween();
        switch (changeType)
        {
            case TileColorChangeType.Highlight:
                _indicatorColorTween = _indicator.DOColor(_indicatorHighlightColor, duration).OnComplete(() => { _indicatorColorTween = null; });
                _fillColorTween = _fill.DOColor(_fillHighlightColor, duration).OnComplete(() => { _fillColorTween = null; });
                break;
            case TileColorChangeType.Reset:
                _indicatorColorTween = _indicator.DOColor(_indicatorColor, duration).OnComplete(() => { _indicatorColorTween = null; });
                _fillColorTween = _fill.DOColor(_fillColor, duration).OnComplete(() => { _fillColorTween = null; });
                break;
        }

    }

    // 기존 진행중인 colorTween을 중지, 삭제
    public void KillColorTween()
    {
        _indicatorColorTween?.Kill();
        _indicatorColorTween = null;

        _fillColorTween?.Kill();
        _fillColorTween = null;
    }

    private void InitSprites()
    {
        _indicator = TileObject.transform.Find("line").GetComponent<SpriteRenderer>();
        _fill = TileObject.transform.Find("fill").GetComponent<SpriteRenderer>();
    }

    // Sprite로 Mesh를 만들고 Collider에 적용: raycast를 위해 필요
    private void InitMesh()
    {
        Mesh mesh = Util.SpriteToMesh(_fill.sprite);
        TileObject.transform.Find("collider").GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public void DestroyIcon()
    {
        if (Icon != null)
            Icon.GetComponent<SpriteRenderer>().DOFade(0, 0.5f).OnComplete(() => { GameObject.Destroy(Icon); });
    }


    public abstract void OnTileEnter();

    public abstract void OnTileEventFinish();
}
