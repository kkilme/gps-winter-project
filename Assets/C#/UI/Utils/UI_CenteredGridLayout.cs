using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GridLayoutGroup이 한 행일 때도 childAlignment = UpperCenter의 정렬을 유지하며 자식들을 좌측 상단부터 배치하도록 강제하는 컴포넌트.
/// </summary>
// 가정: StartCorner = UpperLeft, Axis = Horizontal, Constraint = Flexible
[ExecuteAlways]
[RequireComponent(typeof(GridLayoutGroup))]
public class UI_CenteredGridLayout : MonoBehaviour
{
    private void OnEnable() => UpdateLayout();
    private void OnTransformChildrenChanged() => UpdateLayout();

    private void OnRectTransformDimensionsChange() => UpdateLayout();

#if UNITY_EDITOR
    private void OnValidate() => UpdateLayout();
#endif

    private void UpdateLayout()
    {
        GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
        RectTransform rect = GetComponent<RectTransform>();

        // 활성 자식만 카운트
        int activeChildCount = transform.Cast<Transform>().Count(t => t.gameObject.activeSelf);

        float totalWidth = rect.rect.width;
        float cellWidth = grid.cellSize.x;
        float spacing = grid.spacing.x;

        // totalWidth + spacing: 마지막 셀 뒤는 spacing이 필요하지 않다는 점을 반영
        int itemsPerRow = Mathf.FloorToInt((totalWidth + spacing) / (cellWidth + spacing));
        itemsPerRow = Mathf.Max(itemsPerRow, 1);

        if (activeChildCount >= itemsPerRow)
        {
            grid.childAlignment = TextAnchor.UpperCenter;
            grid.padding.left = 0;
        }
        else
        {
            grid.childAlignment = TextAnchor.UpperLeft;

            float rowPreferredWidth = itemsPerRow * cellWidth + (itemsPerRow - 1) * spacing;
            grid.padding.left = Mathf.RoundToInt((totalWidth - rowPreferredWidth) / 2);
        }
    }
}