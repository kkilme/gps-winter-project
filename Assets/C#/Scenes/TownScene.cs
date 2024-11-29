using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.SceneType.TownScene;

        // UIManager test
        Managers.UIMng.ShowSceneUI<UI_TownScene>();
    }

    public override void Clear()
    {
        #if UNITY_EDITOR
        Debug.Log("TownScene Clear!");
        #endif
    }
}
