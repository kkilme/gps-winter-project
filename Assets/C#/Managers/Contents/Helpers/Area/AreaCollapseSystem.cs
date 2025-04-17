using System.Collections;
using UnityEngine;

// 턴 진행에 따른 Area 붕괴 시스템
public class AreaCollapseSystem
{
    private AreaManager _areaManager => Managers.AreaMng;
    private AreaMap _map => Managers.AreaMng.Map;

    private int _turnCount = 0;
    public int TurnCount
    {
        get => _turnCount;
        set
        {
            _turnCount = value;
            if (TurnCount != 0 && TurnCount % _suddendeathTimer == 0) ProgressSuddendeath();
            else _areaManager.AreaState = AreaState.Idle;
        }
    }
    private int _suddendeathTimer = 4; // timer번의 이동마다 맨 밑 타일 파괴됨. Area별로 다르게 할 수도?
    private int _suddendeathCount = 0;

    private void ProgressSuddendeath()
    {
        // 보스 위치 기준 최대 2칸 아래까지만 파괴됨
        if (_suddendeathCount == _map.BossPosition.y - _map.PlayerStartPosition.y - 2)
        {
            _areaManager.AreaState = AreaState.Idle;
            return;
        }

        _map.DestroyTiles(_suddendeathCount);
        _suddendeathCount++;
        _areaManager.AreaState = AreaState.Idle;
    }
}
