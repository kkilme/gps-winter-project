using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public interface IAreaActionTargetSelector
{
    public void StartTargetSelection(Action<object> onTargetSelected);
    public void CancelTargetSelection();
    public void OnTargetSelected(PointerEventData pointerEventData);
}
