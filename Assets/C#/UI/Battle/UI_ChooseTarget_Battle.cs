using System;
using UnityEngine.UI;

public class UI_ChooseTarget_Battle : UI_Base
{
    enum Buttons
    {
        Button_Cancel,
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        GetButton(Buttons.Button_Cancel).onClick.AddListener(() =>
        {
            Managers.BattleMng.UnsetAction();
            Managers.BattleMng.UI.ActionPanel.ShowInstantly();
        });
    }
}
