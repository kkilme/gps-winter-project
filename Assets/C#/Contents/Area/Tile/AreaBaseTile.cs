using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// Area 맵의 기본 육각형 타일에 붙는 스크립트
public class AreaBaseTile : MonoBehaviour
{   
    // 플레이어가 밟고 서는 타일 오브젝트 그 자체
    private GameObject _tile;
    // 타일 위의 장식물 오브젝트
    private GameObject _decoration;
    public bool IsDecorationEnabled;

    public void Init()
    {
        _tile = gameObject;
        _decoration = gameObject.transform.GetChild(0).gameObject;
        DisableDecoration();
    }

    // 빛의 효과를 받는 타일로 설정
    public void EnableLight()
    {   
        _tile.SetLayerRecursively(LayerMask.NameToLayer("AreaLightTarget"));
    }

    public void SetDecorationEnabled()
    {
        IsDecorationEnabled = true;
        _decoration.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0); // 장식물 각도를 랜덤으로 하여 랜덤성 부여
        EnableDecoration();
    }

    public void EnableDecoration()
    {
        if (!IsDecorationEnabled) return;
        _decoration.SetActive(true);
    }

    public void DisableDecoration()
    {
        _decoration.SetActive(false);
    }
}