using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AreaCameraController : MonoBehaviour
{   
    public bool Freeze { get;  set; } // true: 카메라 정지
    // 실제 카메라가 부착된 오브젝트의 트랜스폼
    private Transform _cameraTransform;

    // 카메라 이동 속도 (이동 속도는 zoom 단계에 따라 조절됨)
    [SerializeField]
    private float _moveSpeedMax;
    [SerializeField]
    private float _moveSpeedMin;
    // zoom 단계에 따라 조정된 moveSpeed 저장하는 배열
    private float[] _moveSpeed;

    [SerializeField]
    // 카메라의 각종 값들이 새 값으로 변경되는 시간 (클수록 더 빨리 변경됨)
    private float _moveTime;
    [SerializeField]
    // 한 번 입력에 카메라 zoom되는 양
    private Vector3Int _zoomAmount;
    [SerializeField]
    // 화면 이동을 위해 필요한 (마우스 위치 - 스크린 모서리 위치) 값
    private float _borderThickness;

    // 카메라가 이동할 수 있는 최대, 최소 x, z (Vector2.x: min, Vector2.y: max)
    [SerializeField]
    private Vector2 _posLimitX;
    [SerializeField]
    private Vector2 _posLimitZ;

    private Vector2 _adjustedPosLimitZ;


    // 카메라 zoom 최대, 최소
    [SerializeField]
    private int _zoomoutLimit;
    [SerializeField]
    private int _zoominLimit;


    // 카메라의 다음 위치
    private Vector3 _newPosition;
    // 카메라의 다음 zoom
    private Vector3Int _newZoom;

    // 마우스 드래그를 통한 화면 이동을 위해 사용되는 변수들
    private Vector3 _dragStartPosition;
    private Vector3 _dragCurrentPosition;
    private Plane _plane;
    private float _entry;

    // 줌 단계에 따라 카메라 이동속도 조정
    private int _zoomLevel;

    void Start()
    {
        _cameraTransform = transform.GetChild(0);
        if (_cameraTransform == null || _cameraTransform.GetComponent<Camera>() == null)
        {
            Debug.LogError("No camera attached to AreaCameraController");
        }

        _newPosition = transform.position;
        _newZoom = new Vector3Int(0, 0, 0);
        _plane = new Plane(Vector3.up, Vector3.zero);
        _entry = 0;
        _adjustedPosLimitZ = new Vector2(_posLimitZ.x, _posLimitZ.y);

        InitializeMoveSpeedWithZoom();
        _zoomLevel = CalculateZoomlevel();

        Managers.InputMng.KeyAction += HandleKeyScreenMove;
        //Managers.InputMng.MouseAction += HandleMouseInput;
    }

    void Update()
    {
        if (Freeze) return;
        HandleZoom();
        HandleMouseInput();
        _zoomLevel = CalculateZoomlevel();
        HandleMouseScreenMove();
        UpdateCamera();
    }

    // 클릭 + 드래그를 통한 카메라 이동
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (_plane.Raycast(ray, out _entry))
            {
                _dragStartPosition = ray.GetPoint(_entry);
            }
        }

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (_plane.Raycast(ray, out _entry))
            {
                _dragCurrentPosition = ray.GetPoint(_entry);
                _newPosition = transform.position + _dragStartPosition - _dragCurrentPosition;
            }
        }
    }

    // Inputmanager 사용한 클릭+드래그로 화면 이동: 제대로 작동하지 않음
    //private void HandleMouseInput(Define.MouseEvent mouseEvent)
    //{
    //    if (mouseEvent == Define.MouseEvent.PointerDown)
    //    {
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        if (_plane.Raycast(ray, out _entry))
    //        {
    //            _dragStartPosition = ray.GetPoint(_entry);
    //        }
    //    }
    //    if (mouseEvent == Define.MouseEvent.Press)
    //    {
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        if (_plane.Raycast(ray, out _entry))
    //        {
    //            _dragCurrentPosition = ray.GetPoint(_entry);
    //            _newPosition = transform.position + _dragStartPosition - _dragCurrentPosition;
    //        }
    //    }
    //}

    // 마우스 휠을 이용한 zoom
    private void HandleZoom()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            _newZoom.y += (int)(Input.mouseScrollDelta.y * _zoomAmount.y);
            _newZoom.z += (int)(Input.mouseScrollDelta.y * _zoomAmount.z);
        }

        _newZoom.y = Mathf.Clamp(_newZoom.y, _zoominLimit, _zoomoutLimit);
        _newZoom.z = Mathf.Clamp(_newZoom.z, -_zoomoutLimit, -_zoominLimit);

        
    }

    // 키보드 입력을 통한 카메라 이동
    private void HandleKeyScreenMove()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.mousePosition.y >= Screen.height - _borderThickness)
        {
            _newPosition += transform.forward * _moveSpeed[_zoomLevel - 1] / 10;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || Input.mousePosition.x <= _borderThickness)
        {
            _newPosition += transform.right * -_moveSpeed[_zoomLevel - 1] / 10;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.mousePosition.y <= _borderThickness)
        {
            _newPosition += transform.forward * -_moveSpeed[_zoomLevel - 1] / 10;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || Input.mousePosition.x >= Screen.width - _borderThickness)
        {
            _newPosition += transform.right * _moveSpeed[_zoomLevel - 1] / 10;
        }
    }

    // 마우스를 스크린 경계 근처로 가져갈 시 카메라 이동
    private void HandleMouseScreenMove()
    {
        if (Input.mousePosition.y >= Screen.height - _borderThickness)
        {
            _newPosition += transform.forward * _moveSpeed[_zoomLevel - 1] / 10;
        }
        if (Input.mousePosition.x <= _borderThickness)
        {
            _newPosition += transform.right * -_moveSpeed[_zoomLevel - 1] / 10;
        }
        if (Input.mousePosition.y <= _borderThickness)
        {
            _newPosition += transform.forward * -_moveSpeed[_zoomLevel - 1] / 10;
        }
        if (Input.mousePosition.x >= Screen.width - _borderThickness)
        {
            _newPosition += transform.right * _moveSpeed[_zoomLevel - 1] / 10;
        }
    }
    // 카메라 위치 및 zoom 업데이트
    private void UpdateCamera()
    {
        _adjustedPosLimitZ = new Vector2( _posLimitZ.x -_zoomAmount.z * (_zoomLevel-1) * 0.6f, _posLimitZ.y - _zoomAmount.z * (_zoomLevel - 1) * 0.3f);
        _newPosition.x = Mathf.Clamp(_newPosition.x, _posLimitX.x, _posLimitX.y);
        _newPosition.z = Mathf.Clamp(_newPosition.z, _adjustedPosLimitZ.x, _adjustedPosLimitZ.y);
        transform.position = Vector3.Lerp(transform.position, _newPosition, _moveTime * Time.deltaTime);
        _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, _newZoom, _moveTime * Time.deltaTime);
    }

    private int CalculateZoomlevel()
    {
        return Mathf.Abs((_zoomoutLimit - _newZoom.y) / _zoomAmount.y) + 1;
    }

    // zoom 단계에 따른 카메라 이동 속도를 미리 계산해서 저장
    private void InitializeMoveSpeedWithZoom()
    {   
        if (_zoominLimit > _zoomoutLimit)
        {
            Debug.LogError("zoominLimit must be smaller than zoomoutLimit!");
            return;
        }
        if ((_zoomoutLimit - _zoominLimit) % _zoomAmount.y != 0 || (_zoomoutLimit - _zoominLimit) % _zoomAmount.z != 0)
        {
            Debug.LogError("(_zoomoutLimit - _zoominLimit) must be divisible by zoomAmount!");
            return;
        }
        _moveSpeed = new float[Mathf.Abs((_zoomoutLimit - _zoominLimit) / _zoomAmount.y) + 1];

        // 1~n단계의 zoom을 moveSpeedMin과 moveSpeedMax사이 값으로 변환
        float LinearTransform(float value, float minOriginal, float maxOriginal, float minNew, float maxNew)
        {
            return ((value - minOriginal) / (maxOriginal - minOriginal)) * (maxNew - minNew) + minNew;
        }
        for (int i = _moveSpeed.Length - 1; i >= 0; i--)
        {
            _moveSpeed[_moveSpeed.Length - 1 - i] = LinearTransform(i + 1, 1, _moveSpeed.Length, _moveSpeedMin, _moveSpeedMax);
        }
    }
}
