using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UI;

public class UI_AreaScene : UI_Scene
{
    enum SubItemUI
    {
        UI_AreaTopBar,
        UI_CoinTossDisplay,
        UI_AreaButtons,
        UI_CollapseInformer,
    }

    enum Images
    {
        FadeBG,
    }

    public UI_AreaTopBar TopBar { get; protected set; }
    public UI_AreaButtons AreaButtons { get; protected set; }
    public UI_CollapseInformer CollapseInformer { get; protected set; }
    public UI_CoinTossDisplay CoinTossDisplay { get; protected set; }

    private AreaManager _areaManager => Managers.AreaMng;

    public override void Init()
    {
        base.Init();
        Bind<UI_Base>(typeof(SubItemUI));
        Bind<Image>(typeof(Images));

        TopBar = Get<UI_Base>(SubItemUI.UI_AreaTopBar).GetOrAddComponent<UI_AreaTopBar>();
        AreaButtons = Get<UI_Base>(SubItemUI.UI_AreaButtons).GetOrAddComponent<UI_AreaButtons>();
        CollapseInformer = Get<UI_Base>(SubItemUI.UI_CollapseInformer).GetOrAddComponent<UI_CollapseInformer>();
        CoinTossDisplay = Get<UI_Base>(SubItemUI.UI_CoinTossDisplay).GetOrAddComponent<UI_CoinTossDisplay>();
    }
}
