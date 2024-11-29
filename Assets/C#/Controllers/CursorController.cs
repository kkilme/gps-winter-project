using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    private Texture2D _attackIcon;
    private Texture2D _handIcon;
    
    private enum CursorType
    {
        None,
        Attack,
        Hand,
    }

    private CursorType _cursorType = CursorType.None;
    
    private int _layerMask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);
    
    void Start()
    {
        _attackIcon = Managers.ResourceMng.Load<Texture2D>("Textures/Cursors/Attack");
        _handIcon = Managers.ResourceMng.Load<Texture2D>("Textures/Cursors/Hand");
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
            return;
	    
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, _layerMask))
        {
            if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
            {
                if (_cursorType == CursorType.Attack)
                    return;
			    
                UnityEngine.Cursor.SetCursor(_attackIcon, new Vector2(_attackIcon.width / 5, 0), CursorMode.Auto);
                _cursorType = CursorType.Attack;
            }
            else
            {
                if (_cursorType == CursorType.Hand)
                    return;
			    
                UnityEngine.Cursor.SetCursor(_handIcon, new Vector2(_handIcon.width / 3, 0), CursorMode.Auto);
                _cursorType = CursorType.Hand;
            }
        }
    }
}
