using System.Collections;
using UnityEngine;

// 카메라 방향을 계속 바라보는 UI
public class UI_Billboard : MonoBehaviour
{
    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        transform.forward = _camera.transform.forward;
    }
}
