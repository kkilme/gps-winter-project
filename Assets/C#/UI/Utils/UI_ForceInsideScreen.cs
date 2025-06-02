using UnityEngine;

/// <summary>
/// 화면 내부를 벗어나지 않아야 하는 UI에 부착. 주로 툴팁 UI에 사용.
/// </summary>
public class UI_ForceInsideScreen : MonoBehaviour
{
    private RectTransform _rect; // 따라다닐 UI의 RectTransform
    private Canvas _canvas; // 이 UI가 속한 캔버스
    private Vector2 _padding = new Vector2(8, 8); // 마우스와 UI의 기본 간격

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    private void LateUpdate()
    {
        SetPosition();
    }

    private void SetPosition()
    {
        if (_rect == null || _canvas == null)
            return;

        Vector2 mouseScreenPos = Input.mousePosition;
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform, mouseScreenPos, _canvas.worldCamera, out anchoredPos);

        Vector2 rectSize = _rect.rect.size * _rect.localScale;
        Vector2 canvasSize = (_canvas.transform as RectTransform).rect.size;

        // 기본적으로 마우스 오른쪽 밑에 ui를 띄움
        Vector2 pivot = new Vector2(0, 1);
        Vector2 offset = new Vector2(_padding.x, -_padding.y);

        // 화면 오른쪽을 벗어나면 왼쪽에 붙임
        if (anchoredPos.x + rectSize.x + _padding.x > canvasSize.x / 2f)
        {
            pivot.x = 1;
            offset.x = -_padding.x;
        }
        // 화면 왼쪽을 벗어나면 오른쪽에 붙임
        else if (anchoredPos.x - rectSize.x - _padding.x < -canvasSize.x / 2f)
        {
            pivot.x = 0;
            offset.x = _padding.x;
        }
        // 화면 위를 벗어나면 아래에 붙임
        if (anchoredPos.y + _padding.y > canvasSize.y / 2f)
        {
            pivot.y = 0;
            offset.y = _padding.y;
        }
        // 화면 아래를 벗어나면 위에 붙임
        else if (anchoredPos.y - rectSize.y - _padding.y < -canvasSize.y / 2f)
        {
            pivot.y = 1;
            offset.y = -_padding.y;
        }

        _rect.pivot = pivot;
        Vector2 targetPos = anchoredPos + offset;
        // ui가 화면을 벗어나지 않도록 위치를 강제 조정
        float minX = -canvasSize.x / 2f + rectSize.x * _rect.pivot.x;
        float maxX = canvasSize.x / 2f - rectSize.x * (1 - _rect.pivot.x);
        float minY = -canvasSize.y / 2f + rectSize.y * _rect.pivot.y;
        float maxY = canvasSize.y / 2f - rectSize.y * (1 - _rect.pivot.y);

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        _rect.anchoredPosition = targetPos;
    }
}