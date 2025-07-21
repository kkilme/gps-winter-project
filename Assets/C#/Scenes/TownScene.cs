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
        if (Managers.SceneMng.LastSceneType == SceneType.UnknownScene)
        {
            Managers.SoundMng.PlayBGM("TownBGM");
            Managers.TownMng.Init();
        }
    }
}
