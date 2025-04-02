using UnityEngine;

public sealed class BattleTile : AreaEventTile
{
    public override void OnTileEnter()
    {   
        Managers.AreaMng.LoadBattleScene();
    }

    public override void OnTileEventFinish()
    {

    }
}
