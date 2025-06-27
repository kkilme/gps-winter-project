using System.Collections;
using UnityEngine;


public static class UIUtility
{
    public enum RectPosDirection
    {
        Top,
        Bottom,
        Left,
        Right
    }

    /// <summary>
    /// reference를 기준으로 target을 reference와 겹치지 않는 특정 방향의 적절한 위치에 위치시킨다.
    /// </summary>
    /// <remarks>reference와 target의 피벗이 (0.5, 0.5)라고 가정함에 주의.</remarks>
    /// <param name="reference">기준이 되는 RectTransform</param>
    /// <param name="target">위치를 설정할 RectTransform</param>
    /// <param name="direction">target RectTransform이 reference에 대해 위치할 방향(위/아래/왼쪽/오른쪽)</param>
    /// <param name="offset">기준 위치로부터 더 떨어뜨릴 거리</param>
    public static void SetRectPositionRelativeTo(RectTransform reference, RectTransform target, RectPosDirection direction, Vector2 offset = default)
    {
        Vector3 refCenter = GetRectCenterWorldPosition(reference); // reference의 중심 위치를 월드 좌표로 가져옴

        Vector3 refSize = reference.rect.size * reference.lossyScale; // reference의 크기

        // target의 크기
        Vector2 targetSize = target.rect.size * target.lossyScale;

        // target을 배치할 World 위치 계산
        // reference와 target의 피벗이 (0.5, 0.5)라고 가정하므로, 그렇지 않은 경우 고려할 시 수정 필요.
        Vector3 basePos = Vector3.zero;
        switch (direction)
        {
            case RectPosDirection.Top:
                basePos = refCenter + new Vector3(0, targetSize.y * 0.5f + refSize.y * 0.5f, 0);
                break;
            case RectPosDirection.Bottom:
                basePos = refCenter - new Vector3(0, targetSize.y * 0.5f + refSize.y * 0.5f, 0);
                break;
            case RectPosDirection.Left:
                basePos = refCenter - new Vector3(targetSize.x * 0.5f + refSize.x * 0.5f, 0, 0);
                break;
            case RectPosDirection.Right:
                basePos = refCenter + new Vector3(targetSize.x * 0.5f + refSize.x * 0.5f, 0, 0);
                break;
        }

        // 오프셋 적용
        basePos += (Vector3)offset;

        Vector3 localPos = basePos;
        if(target.parent != null) localPos = target.parent.InverseTransformPoint(basePos); // 부모가 있다면 로컬 좌표로 변환

        //Debug.Log($"Setting position for {target.name} relative to {reference.name} at {localPos} in {direction} direction with offset {offset}.");

        // 원래 앵커를 유지하기 위한 임시 변수
        Vector2 originalAnchorMin = target.anchorMin;
        Vector2 originalAnchorMax = target.anchorMax;

        // localPos는 앵커가 (0, 0)인 상태에서의 위치이므로, 앵커를 (0, 0)으로 설정
        target.anchorMin = Vector2.zero;
        target.anchorMax = Vector2.zero;

        // 위치 설정
        target.anchoredPosition = localPos;

        // 현재 위치 유지용 변수
        Vector3 worldPos = target.position;

        // 원래 앵커로 되돌림
        target.anchorMin = originalAnchorMin;
        target.anchorMax = originalAnchorMax;

        // 위치 유지
        Vector3 correctedLocalPos = target.parent.InverseTransformPoint(worldPos);
        target.anchoredPosition = correctedLocalPos;
    }

    public static void SetRectPositionRelativeTo(GameObject reference, GameObject target, RectPosDirection direction, Vector2 offset = default)
    {
        RectTransform referenceRect = reference.GetComponent<RectTransform>();
        RectTransform targetRect = target.GetComponent<RectTransform>();
        if(referenceRect == null || targetRect == null)
        {
            Debug.LogError("Both GameObjects must have RectTransform components.");
            return;
        }
        SetRectPositionRelativeTo(referenceRect, targetRect, direction, offset);
    }

    /// <summary>
    /// RectTransform의 중심 위치를 월드 좌표로 반환. Anchor와 Pivot에 관계없이 RectTransform의 실제 중심을 계산한다.
    /// </summary>
    public static Vector3 GetRectCenterWorldPosition(RectTransform rect)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        return (corners[0] + corners[2]) * 0.5f;
    }
}
