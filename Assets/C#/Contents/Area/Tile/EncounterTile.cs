using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public sealed class EncounterTile : AreaEventTile
{
    public override void OnTileEnter()
    {
        Managers.SceneMng.GetCurrentScene<AreaScene>().AreaState = Define.AreaState.Idle; // TODO - Encounter 구현 시 상태 수정
    }

    public override void OnTileEventFinish()
    {
        throw new System.NotImplementedException();
    }
}
