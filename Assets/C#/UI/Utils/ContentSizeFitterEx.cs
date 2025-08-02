using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ContentSizeFitter 컴포넌트에 최대 크기를 제한하는 기능을 추가한 컴포넌트.
/// </summary>
public class ContentSizeFitterEx : ContentSizeFitter
{
    public bool LimitWidth = false;
    public bool LimitHeight = false;
    public float MaxWidth = 0;
    public float MaxHeight = 0;

    public override void SetLayoutHorizontal()
    {
        base.SetLayoutHorizontal();
        if (!LimitWidth) return;

        var rectTransform = transform as RectTransform;
        var sizeDelta = rectTransform.sizeDelta;

        sizeDelta.x = Mathf.Min(sizeDelta.x, MaxWidth);

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeDelta.x);
    }

    public override void SetLayoutVertical()
    {
        base.SetLayoutVertical();
        if (!LimitHeight) return;

        var rectTransform = transform as RectTransform;
        var sizeDelta = rectTransform.sizeDelta;

        sizeDelta.y = Mathf.Min(sizeDelta.y, MaxHeight);

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeDelta.y);
    }

}

[CustomEditor(typeof(ContentSizeFitterEx))]
public class ContentSizeFitterExEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}