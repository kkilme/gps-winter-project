using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemContent : UI_Base
{
    enum Images
    {
        ItemIcon,
    }

    enum Text
    {
        ItemName,
        ItemPrice
    }

    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Text));
    }

    public void Init(TestItem testItem)
    {
        GetText(Text.ItemName).text = testItem.Name;
        GetText(Text.ItemPrice).text = testItem.Price.ToString();
        if (testItem.Icon != null)
            Get<Image>(Images.ItemIcon).sprite = testItem.Icon;
    }
}
