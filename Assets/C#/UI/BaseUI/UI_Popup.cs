using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup : UI_Base
{
    public override void Init()
    {
        Managers.UIMng.SetCanvas(gameObject, true);
    }

    // Popup 닫기
    public virtual void ClosePopupUI()
    {
        Managers.UIMng.ClosePopupUI(this);
    }
}
