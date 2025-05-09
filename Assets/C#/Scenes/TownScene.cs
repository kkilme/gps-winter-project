using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = SceneType.TownScene;
    }

    private void Start()
    {
        // TODO: 첫 게임 시작 시의 영웅 파티 구성 
        if(Managers.SceneMng.FirstScene is TownScene) Managers.HeroMng.AddHeroesOnTest();

        Managers.TownMng.Init();
    }
}
