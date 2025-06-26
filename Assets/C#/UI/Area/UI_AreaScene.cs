using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_AreaScene : UI_Scene
{
    enum SubItemUI
    {
        UI_AreaTopBar,
        UI_CoinTossDisplay,
        UI_AreaButtons,
        UI_CollapseInformer,
        UI_HeroProfileGroup_Horizontal,
    }

    enum Images
    {
        FadeBG,
    }

    public UI_AreaTopBar TopBar { get; protected set; }
    public UI_AreaButtons AreaButtons { get; protected set; }
    public UI_CollapseInformer CollapseInformer { get; protected set; }
    public UI_CoinTossDisplay CoinTossDisplay { get; protected set; }
    public UI_HeroProfileGroup HeroProfileGroupUI { get; protected set; }

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(SubItemUI));
        Bind<Image>(typeof(Images));

        AreaButtons = GetGameObject(SubItemUI.UI_AreaButtons).GetOrAddComponent<UI_AreaButtons>();
        TopBar = GetGameObject(SubItemUI.UI_AreaTopBar).GetOrAddComponent<UI_AreaTopBar>();
        CollapseInformer = GetGameObject(SubItemUI.UI_CollapseInformer).GetOrAddComponent<UI_CollapseInformer>();
        CoinTossDisplay = GetGameObject(SubItemUI.UI_CoinTossDisplay).GetOrAddComponent<UI_CoinTossDisplay>();
        HeroProfileGroupUI = GetGameObject(SubItemUI.UI_HeroProfileGroup_Horizontal).GetOrAddComponent<UI_HeroProfileGroup>();
    }

    // AreaManager의 Init 완료 후 호출
    public void OnAreaInitComplete()
    {
        CoinTossDisplay.Hide();
        AreaButtons.Show();
        TopBar.Show();
        CollapseInformer.Show();
        HeroProfileGroupUI.BindCreature();
        HeroProfileGroupUI.Show();
    }
}