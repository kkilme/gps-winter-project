using DG.Tweening;
using System.Collections;
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

    public IEnumerator ProgressTurn(int turnCount = 1)
    {
        if (_collapseFinished)
        {
            _turnCount += turnCount;
            yield break;
        }

        for(int i = 0; i<turnCount; i++)
        {
            _turnCount++;
            if (_turnCount % _collapseTimer == 0)
            {
                ProgressCollapse();
            }
            _collapseInformer.ProgressTimer();
            if(turnCount > 1) yield return new WaitForSeconds(0.2f);
        }

        // 보스 위치로부터 최대 2칸 아래까지만 파괴됨
        if (_collapseCount * _collapseAmount >= _map.PlayableFieldHeight - 2)
        {
            _collapseFinished = true;
            _collapseInformer.OnCollapseFinished();
            Debug.Log("[AreaCollapseSystem] Collapse is at limit.");
        }
    }

    public void ProgressCollapse()
    {
        _map.CollapseTiles(_collapseCount * _collapseAmount, _collapseAmount);
        _collapseCount++;
        Debug.Log($"[AreaCollapseSystem] ProgressCollapse - TurnCount: {_turnCount}, CollapseCount: {_collapseCount}");
    }
}
