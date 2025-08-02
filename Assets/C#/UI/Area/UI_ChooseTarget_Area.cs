using System;
using UnityEngine.UI;

public class UI_ChooseTarget_Area : UI_Base
{
    enum Buttons
    {
        Button_Cancel,
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
    }

    public void BindCancelEvent(Action cancelAction)
    {
        GetButton(Buttons.Button_Cancel).onClick.RemoveAllListeners();
        GetButton(Buttons.Button_Cancel).onClick.AddListener(() =>
        {
            cancelAction?.Invoke();
            HideInstantly();
        });
    }

    public override void HideInstantly()
    {
        Managers.AreaMng.UI.HideScreenDim();
        base.HideInstantly();
    }

    private void OnDestroy()
    {
        GetButton(Buttons.Button_Cancel).onClick.RemoveAllListeners();
    }
}
