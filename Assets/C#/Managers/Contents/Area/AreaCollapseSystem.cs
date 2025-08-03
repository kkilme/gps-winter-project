using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 턴 진행에 따른 Area 붕괴 시스템
public class AreaCollapseSystem
{
    private AreaManager _areaManager => Managers.AreaMng;
    private AreaMap _map => Managers.AreaMng.Map;
    private UI_CollapseInformer _collapseInformer => _areaManager.UI.CollapseInformer;

    private int _turnCount = 0;
    private int _collapseTimer;
    private int _collapseAmount;
    private int _collapseCount = 0;
    private bool _collapseFinished = false;

    public void Init(int collapseTimer, int collapseAmount)
    {
        _collapseAmount = collapseAmount;
        _collapseTimer = collapseTimer;
        _collapseInformer.BindData(_collapseTimer, _collapseAmount);
    }

    /// <summary>
    /// progressCount만큼의 붕괴 진척도 진행 
    /// </summary>
    public IEnumerator ProgressCollapse(int progressCount = 1)
    {
        if (_collapseFinished)
        {
            _turnCount += progressCount;
            yield break;
        }

        for (int i = 0; i < progressCount; i++)
        {
            _turnCount++;
            if (_turnCount % _collapseTimer == 0)
            {
                ExecuteCollapse();
            }
            var tween = _collapseInformer.ProgressTimer();
            if (progressCount > 1) yield return tween.WaitForCompletion();
        }

        // 보스 위치로부터 최대 2칸 아래까지만 파괴됨
        if (_collapseCount * _collapseAmount >= _map.PlayableFieldHeight)
        {
            _collapseFinished = true;
            _collapseInformer.OnCollapseFinished();
            Debug.Log("[AreaCollapseSystem] Collapse is at limit.");
        }
    }

    /// <summary>
    /// 붕괴 진척도가 다 찰 시 실제 붕괴 진행
    /// </summary>
    public void ExecuteCollapse()
    {
        List<Vector2Int> collapsedTiles = _map.CollapseTiles(_collapseCount * _collapseAmount, _collapseAmount);
        _map.WorldToGridPosition(_areaManager.CurrentPlayerPosition, out int x, out int z);

        // 현재 플레이어 위치가 붕괴된 타일에 포함되어 있다면 데미지 주기
        if (collapsedTiles.Contains(new Vector2Int(x, z)))
        {
            CollapsedTile collapsedTile = _map.EventTileMap[z, x] as CollapsedTile;
            if(collapsedTile != null)
            {
                collapsedTile.ApplyCollapseDamage();
            }
        }
        _collapseCount++;
    }
}
