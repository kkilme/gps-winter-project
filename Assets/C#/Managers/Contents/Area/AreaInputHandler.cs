using UnityEngine;

// Area에서의 키보드/마우스 인풋 처리
public class AreaInputHandler
{
    private AreaManager _areaManager => Managers.AreaMng;
    private AreaCameraController _cameraController => _areaManager.CameraController;
    private AreaMap _map => _areaManager.Map;

    private Vector3 _currentMouseoverPosition; // 현재 마우스 위치의 WorldPosition
    private GameObject _mouseoverIndicator; // 마우스 위치의 타일 강조해주는 육각형 테두리 형태 게임오브젝트

    public void Init(Vector3 playerStartPos)
    {
        _mouseoverIndicator = Managers.ResourceMng.Instantiate("Area/mouseover_indicator", playerStartPos);
    }


    public void HandleMouseInput(MouseEvent mouseEvent)
    {
        if (_areaManager.AreaState != AreaState.Idle) return;

        // mouseoverIndicator 위치 조정
        if (_cameraController.GetMouseoverPosition(out Vector3 mouseOverPosition) && _map.IsPositionStandable(mouseOverPosition))
        {
            _mouseoverIndicator.transform.position = _map.GetTileCenterPosition(mouseOverPosition);
            _currentMouseoverPosition = mouseOverPosition;
        }
        else return;

        switch (mouseEvent)
        {
            case MouseEvent.PointerUp:
                _areaManager.UI.ResetUI();
                _areaManager.MoveHeroes(_currentMouseoverPosition);
                break;
        }
    }

    public void Clear()
    {
        Managers.InputMng.RemoveMouseAction(HandleMouseInput);
    }
}
