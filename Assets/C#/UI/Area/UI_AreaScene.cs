using DG.Tweening;
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
        UI_ChooseTarget_Area,
    }

    enum Images
    {
        ScreenDim_UnderHeroProfile,
        ScreenDim_Top,
    }

    public UI_AreaTopBar TopBar { get; private set; }
    public UI_AreaButtons AreaButtons { get; private set; }
    public UI_CollapseInformer CollapseInformer { get; private set; }
    public UI_CoinTossDisplay CoinTossDisplay { get; private set; }
    public UI_HeroProfileGroup HeroProfileGroupUI { get; private set; }
    public UI_ChooseTarget_Area ChooseTargetUI { get; private set; }

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
        ChooseTargetUI = GetGameObject(SubItemUI.UI_ChooseTarget_Area).GetOrAddComponent<UI_ChooseTarget_Area>();
    }

    // AreaManager의 Init 완료 후 호출
    public void OnAreaInitComplete()
    {
        CoinTossDisplay.Hide();
        AreaButtons.Show();
        TopBar.SetAreaName(Managers.AreaMng.AreaData.Name);
        TopBar.Show();
        CollapseInformer.Show();
        HeroProfileGroupUI.BindCreature();
        HeroProfileGroupUI.Show();
        ChooseTargetUI.HideInstantly();
    }

    public UI_ChooseTarget_Area ShowChooseTargetUI(bool showScreenDim = false)
    {
        ChooseTargetUI.Show();
        if (showScreenDim)
            HighlightHeroProfiles();
        return ChooseTargetUI;
    }

    public void ResetUI()
    {
        CoinTossDisplay.Hide();
        ChooseTargetUI.Hide();
        AreaButtons.CloseAllPopups();
        DehighlightHeroProfiles();
        HideScreenDimTop();
    }

    public void HighlightHeroProfiles()
    {
        // ScreenDim 이미지를 통해 HeroProfileGroupUI를 강조하는 효과를 줌.
        Image screenDim = GetImage(Images.ScreenDim_UnderHeroProfile);
        screenDim.DOFade(0.9f, 0.5f);
    }

    public void DehighlightHeroProfiles()
    {
        Image screenDim = GetImage(Images.ScreenDim_UnderHeroProfile);
        screenDim.DOFade(0f, 0.5f);
    }

    public void ShowScreenDimTop()
    {
        // 모든 UI를 덮는 ScreenDim
        Image screenDim = GetImage(Images.ScreenDim_Top);
        screenDim.DOFade(0.9f, 0.5f);
    }

    public void HideScreenDimTop()
    {
        Image screenDim = GetImage(Images.ScreenDim_Top);
        screenDim.DOFade(0f, 0.5f);
    }
}