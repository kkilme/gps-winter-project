using UnityEngine;

public sealed class BattleTile : AreaEventTile
{
    public override void OnTileEnter()
    {   
        // Monobehaviour가 아니기 때문에 이곳에서 직접 코루틴을 시작할 수 없음
        Managers.SceneMng.GetCurrentScene<AreaScene>().LoadBattleScene();
    }

    public override void OnTileEventFinish()
    {

    }
}
