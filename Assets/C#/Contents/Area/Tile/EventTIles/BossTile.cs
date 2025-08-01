using UnityEngine;

public sealed class BossTile : AreaEventTile
{
    public override void OnTileEnter()
    {
        if (_willBeDestroyed) return; // 타일이 파괴될 예정이라면 이벤트를 실행하지 않음
        Managers.AreaMng.LoadBattleScene(BattleType.Boss);
    }
}
