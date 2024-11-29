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

public abstract class AreaGridTile
{
    // 스프라이트 등을 갖는 이 타일에 해당하는 게임 오브젝트 (Name: grid_hex)
    public GameObject TileObject { get; private set; }

    public AreaTileType TileType { get; protected set; }

    protected Vector3 _worldPosition;
    
    // 셀 모서리 스프라이트
    protected SpriteRenderer _indicator;
    // 셀 내부 스프라이트
    protected SpriteRenderer _fill;

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

    protected Color _indicatorColor; // 각 tile별 지정된 색
    protected Color _fillColor;
    protected Color _indicatorHighlightColor; // 플레이어의 이동 가능 지점을 보여줄 때 하이라이트되는 색
    protected Color _fillHighlightColor;


    // DoTween을 통해 스프라이트 색을 바꾸는데, 이 작업을 의미함.
    // 이를 통해 작업을 도중에 취소할 수 있음
    private Tweener _indicatorColorTween;
    private Tweener _fillColorTween;

    private const string GRID_TILE_PATH = "Area/grid_hex";

    // tileObject를 생성자에 줄 시, 기존의 TileObject 오브젝트 재활용. 타일의 type을 바꾸는 데 사용됨. (AreaGrid의 ChangeTile 참조)
    protected AreaGridTile(Vector3 position, GameObject tileObject = null)
    {   
        _worldPosition = position;
        if (tileObject == null)
        {
            InitTileObject();
            InitSprites();
            InitMesh();
        }
        else
        {
            TileObject = tileObject;
            InitSprites();
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

    private void InitTileObject()
    {
        Transform tileParent = GameObject.Find("Tiles").transform;
        TileObject = Managers.ResourceMng.Instantiate(GRID_TILE_PATH, tileParent);
        TileObject.transform.position = _worldPosition;
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
