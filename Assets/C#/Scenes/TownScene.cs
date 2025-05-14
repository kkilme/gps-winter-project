using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownScene : BaseScene
{
    protected override void Init()
    { 
        SceneType = SceneType.TownScene;
        base.Init();
    }

    private void Start()
    {
        // TODO: 첫 게임 시작 시의 영웅 파티 구성 
        Managers.TownMng.Init();
    }
}
