using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChooseTarget : UI_Base
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
        });
    }
}
