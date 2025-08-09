using UnityEngine.UI;
using TMPro;

public class UI_AreaTopBar : UI_Base
{
    enum Buttons
    {
        Button_Menu,
    }

    enum Texts
    {
        Text_AreaName,
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    public void SetAreaName(string areaName)
    {
        GetText(Texts.Text_AreaName).text = areaName;
    }
}
