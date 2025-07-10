using System;
using UnityEngine.EventSystems;

/// <summary>
/// Area에서의 타겟 선택을 위한 인터페이스
/// </summary>
public interface IAreaActionTargetSelector
{
    /// <summary>
    /// 타겟 선택을 시작한다. 타겟이 선택되면 onTargetSelected 콜백이 호출된다.
    /// </summary>
    public void StartTargetSelection(Action<object> onTargetSelected);

    /// <summary>
    /// 타겟 선택 시작 이후, 사용자가 타겟 선택을 취소할 때 호출된다.
    /// </summary>
    public void CancelTargetSelection();

    /// <summary>
    /// 타겟이 선택되었을 때 호출되어 실질적인 onTargetSelected 콜백 실행 및 UI 정리 작업 등을 수행한다.
    /// </summary>
    public void OnTargetSelected(PointerEventData pointerEventData);
}
