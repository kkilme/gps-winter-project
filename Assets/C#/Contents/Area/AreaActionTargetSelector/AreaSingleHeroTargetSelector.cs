using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Area에서 한 영웅을 타겟으로 선택하는 클래스
/// </summary>
public class AreaSingleHeroTargetSelector : IAreaActionTargetSelector
{
    private Action<object> _onTargetSelected;
    private AreaManager AreaManager => Managers.AreaMng;
    public void StartTargetSelection(Action<object> onTargetSelected)
    {
        _onTargetSelected = onTargetSelected;
        AreaManager.AreaState = AreaState.Busy;

        UI_ChooseTarget_Area chooseTargetUI = AreaManager.UI.ShowChooseTargetUI(showScreenDim: true);
        chooseTargetUI.BindCancelEvent(CancelTargetSelection);

        UI_HeroProfileGroup heroProfileGroup = AreaManager.UI.HeroProfileGroupUI;
        heroProfileGroup.StartBlinking();
        heroProfileGroup.BindEvent(OnTargetSelected, UIEvent.Click);
    }

    public void CancelTargetSelection()
    {
        CleanUp();
    }

    public void OnTargetSelected(PointerEventData pointerEventData)
    {
        var go = pointerEventData.pointerCurrentRaycast.gameObject;

        UI_HeroProfile heroProfile = go.GetComponentInParent<UI_HeroProfile>();
        _onTargetSelected.Invoke(heroProfile.BindingCreature);
        CleanUp();
    }

    private void CleanUp()
    {
        AreaManager.UI.HeroProfileGroupUI.StopBlinking();
        AreaManager.UI.HeroProfileGroupUI.ClearEvent();
        AreaManager.UI.HideChooseTargetUI();
        AreaManager.AreaState = AreaState.Idle;
    }
}
