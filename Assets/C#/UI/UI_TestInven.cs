using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TestInven : UI_Scene
{
    enum GameObjects
    {
        GridPanel
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));

        GameObject gridPanel = GetGameObject(GameObjects.GridPanel);
        foreach (Transform child in gridPanel.transform)
            Managers.ResourceMng.Destroy(child.gameObject);

        for (int i = 0; i < 8; i++)
        {
            GameObject go = Managers.UIMng.MakeSubItemUI<UI_TestInven_TestItem>(gridPanel.transform).gameObject;
            UI_TestInven_TestItem invenItem = go.GetOrAddComponent<UI_TestInven_TestItem>();
            invenItem.SetInfo($"ItemData{i}");
        }
    }
}
