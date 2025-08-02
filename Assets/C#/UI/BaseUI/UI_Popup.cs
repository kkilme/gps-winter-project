using UnityEngine;

public class UI_Popup : UI_Base
{
    [SerializeField] private RectTransform _panel;
    /// <summary>
    /// Popup UI의 실제 컨텐츠를 담고 있는 RectTransform
    /// </summary>
    public RectTransform Panel => _panel;

    public override void Init()
    {
        if (_panel == null)
        {
            if (transform.childCount > 0)
            {
                _panel = transform.GetChild(0).GetComponent<RectTransform>();
            }
            else
            {
                Debug.LogWarning($"[UI_Popup] Could not set popup panel of ui: {gameObject.name}");
            }
        }
        Managers.UIMng.SetCanvas(gameObject, true);
    }

    // Popup 닫기
    public virtual void Close()
    {
        Managers.UIMng.ClosePopupUI(this);
    }
}
