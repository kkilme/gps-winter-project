using UnityEngine;

public sealed class BossTile : AreaEventTile
{
    public override void OnTileEnter()
    {
        Managers.AreaMng.AreaState = AreaState.Idle; // TODO - Boss 구현 시 상태 수정
    }

    public override void OnTileEventFinish()
    {
        throw new System.NotImplementedException();
    }
}
