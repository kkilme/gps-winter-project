using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AreaCameraController : MonoBehaviour
{
    #region field
    public bool Freeze { get;  set; } // true: 카메라 정지

    private Camera _camera;
    private Transform _cameraTransform; // 실제 카메라 컴포넌트가 부착된 오브젝트의 트랜스폼
    private float _cameraRotation;

    // 카메라 이동 속도 최대, 최소 (이동 속도는 zoom 단계에 따라 조절됨)
    [SerializeField] private float _moveSpeedMin;
    [SerializeField] private float _moveSpeedMax;
    private float[] _moveSpeed; // zoom 단계에 따라 조정된 moveSpeed 저장하는 배열

    // 카메라 zoom 최대, 최소
    [SerializeField] private int _zoomoutLimit;
    [SerializeField] private int _zoominLimit;

    [SerializeField] private float _moveTime; // 카메라의 각종 값들이 새 값(_newPosition, _newZoom)으로 변경되는 시간 (클수록 더 빨리 변경됨)
    [SerializeField] private int _zoomAmount; // 한 번의 스크롤 입력에 zoom되는 양
    [SerializeField] private float _borderThickness; // 화면 이동을 위해 필요한(마우스 위치 - 스크린 모서리 위치) 값

    // 카메라 위치 제한값들
    [SerializeField, ReadOnly] private float _posLimitXmin;
    [SerializeField, ReadOnly] private float _posLimitXmax;
    [SerializeField, ReadOnly] private float _posLimitZmin;
    [SerializeField, ReadOnly] private float _posLimitZmax;
    private float _bossZ; // 카메라 Z 제한 값 설정에 필요
    private float _startZ; // 카메라 Z 제한 값 설정에 필요

    // 카메라의 다음 위치
    [SerializeField, ReadOnly] private Vector3 _newPosition;
    // 카메라의 다음 zoom
    [SerializeField, ReadOnly] private int _newZoom;

    // 마우스 드래그를 통한 화면 이동을 위해 사용되는 필드들
    private Vector3 _dragStartPosition;
    private Vector3 _dragCurrentPosition;
    private Plane _plane;
    private float _entry;

    // 현재 줌 단계. 이에 따라 카메라 이동속도 조정
    private int _zoomLevel;

    private const float TILE_PREFAB_THICKNESS = 1.0f;
    private const float Y_COORDINATE = 50.0f; // 본 스크립트가 부착된 오브젝트의 y좌표는 고정
    #endregion

    public void Init()
    {
        _camera = GetComponentInChildren<Camera>();
        _cameraRotation = _camera.transform.rotation.eulerAngles.x;
        _cameraTransform = _camera.gameObject.transform;
        _newPosition = transform.position;
        _newZoom = _zoomoutLimit;

        _plane = new Plane(Vector3.up, Vector3.zero);
        _entry = 0;

        InitializeMoveSpeedWithZoom();
        CalculateZoomlevel();

        Managers.InputMng.KeyAction += HandleKeyScreenMove;
    }

    void Update()
    {
        if (Freeze) return;
        HandleZoom();
        HandleMouseInput();
        CalculateZoomlevel();
        HandleMouseScreenMove();
        CalculateZLimit();
        UpdateCamera();
    }

    public void InitPosLimit(float xmin, float xmax, float startPositionZ, float bossPositionZ)
    {
        _posLimitXmin = xmin;
        _posLimitXmax = xmax;
        _startZ = startPositionZ;
        _bossZ = bossPositionZ;

        CalculateZLimit();
        gameObject.transform.position = new Vector3((xmin + xmax) / 2, Y_COORDINATE, _posLimitZmin); // 카메라 시작 위치
    }

    // 카메라의 Z 제한값 설정
    private void CalculateZLimit()
    {
        float cameraHeight = gameObject.transform.position.y + _cameraTransform.localPosition.y - TILE_PREFAB_THICKNESS;
        float angle = Mathf.Deg2Rad * (90 - _cameraRotation);

        _posLimitZmin = _startZ - cameraHeight * Mathf.Tan(angle);
        _posLimitZmax = _bossZ - cameraHeight * Mathf.Tan(angle);
    }

    public bool GetMouseoverPosition(out Vector3 mouseOverPosition)
    {
        mouseOverPosition = Vector3.zero;

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: 100f, layerMask: LayerMask.GetMask("AreaGrid")))
        {
            mouseOverPosition = rayHit.point;

            Debug.DrawLine(_camera.transform.position, rayHit.point, Color.red);

            return true;
        }

        return false;
    }

    // 클릭 + 드래그를 통한 카메라 이동
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (_plane.Raycast(ray, out _entry))
            {
                _dragStartPosition = ray.GetPoint(_entry);
            }
        }

        if (Input.GetMouseButton(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (_plane.Raycast(ray, out _entry))
            {
                _dragCurrentPosition = ray.GetPoint(_entry);
                _newPosition = transform.position + _dragStartPosition - _dragCurrentPosition;
            }
        }
    }
    
    // 마우스 휠을 이용한 zoom
    private void HandleZoom()
    {   
        if (Input.mouseScrollDelta.y != 0)
        {
            _newZoom += (int)(Input.mouseScrollDelta.y * -_zoomAmount);
        }

        _newZoom = Mathf.Clamp(_newZoom, _zoominLimit, _zoomoutLimit);

        // legacy: y, z값을 통한 Zoom ////////////////////////////////////////////////////
        //if (Input.mouseScrollDelta.y != 0)
        //{
        //    _newZoom.y += (int)(Input.mouseScrollDelta.y * -_zoomAmount);
        //    _newZoom.z += (int)(Input.mouseScrollDelta.y * _zoomAmount);
        //}

        //_newZoom.y = Mathf.Clamp(_newZoom.y, _zoominLimit, _zoomoutLimit);
        //_newZoom.z = Mathf.Clamp(_newZoom.z, -_zoomoutLimit, -_zoominLimit);
        /////////////////////////////////////////////////////////////////////////////////
    }

    // 키보드 입력을 통한 카메라 이동
    private void HandleKeyScreenMove()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            _newPosition += transform.forward * _moveSpeed[_zoomLevel - 1] / 10;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            _newPosition += transform.right * -_moveSpeed[_zoomLevel - 1] / 10;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            _newPosition += transform.forward * -_moveSpeed[_zoomLevel - 1] / 10;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
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
        _newPosition.x = Mathf.Clamp(_newPosition.x, _posLimitXmin, _posLimitXmax);
        _newPosition.z = Mathf.Clamp(_newPosition.z, _posLimitZmin, _posLimitZmax);

        transform.position = Vector3.Lerp(transform.position, _newPosition, _moveTime * Time.deltaTime);
        if(Vector3.Distance(transform.position, _newPosition) < 1e-3) transform.position = _newPosition;

        _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _newZoom, _moveTime * Time.deltaTime);

        // legacy: y, z값을 통한 Zoom ////////////////////////////////////////////////////////////////////////////////////////////
        //_cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, _newZoom, _moveTime * Time.deltaTime);
        //if (Vector3.Distance(_cameraTransform.localPosition, _newZoom) < 1e-3) _cameraTransform.localPosition = _newZoom;
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }

    private void CalculateZoomlevel()
    {
        _zoomLevel = Mathf.Abs((_zoomoutLimit - _newZoom) / _zoomAmount) + 1;
    }

    // zoom 단계에 따른 카메라 이동 속도를 미리 계산해서 저장
    private void InitializeMoveSpeedWithZoom()
    {   
        if (_zoominLimit > _zoomoutLimit)
        {
            Debug.LogError("zoominLimit must be smaller than zoomoutLimit!");
            return;
        }
        if ((_zoomoutLimit - _zoominLimit) % _zoomAmount != 0)
        {
            Debug.LogError("(_zoomoutLimit - _zoominLimit) must be divisible by zoomAmount!");
            return;
        }
        _moveSpeed = new float[Mathf.Abs((_zoomoutLimit - _zoominLimit) / _zoomAmount) + 1];

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
