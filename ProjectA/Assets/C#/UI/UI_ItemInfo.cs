using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemInfo : UI_Popup
{
    public RectTransform RectTransform {  get; private set; }

    enum GameObjects
    {
        Panel,
    }

    enum Images
    {
        ItemIcon,
    }

	enum Text
	{
		ItemName,
		ItemDescription,
	}

    public override void Init()
    {
        base.Init();

        Bind<TextMeshProUGUI>(typeof(Text));
        Bind<GameObject>(typeof(GameObjects));
        RectTransform = GetGameObject(GameObjects.Panel).GetOrAddComponent<RectTransform>();
    }

    public void Init(TestItem testItem)
    {
        GetText(Text.ItemName).text = testItem.Name;
        GetText(Text.ItemDescription).text = testItem.Description;
        if (testItem.Icon != null)
            Get<Image>(Images.ItemIcon).sprite = testItem.Icon;
    }
}
