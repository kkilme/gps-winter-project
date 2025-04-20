using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

// Area 맵의 육각형 타일 게임으보젝트에 붙는 스크립트
public class AreaBaseTile : MonoBehaviour
{   
    // 플레이어가 밟고 서는 타일 오브젝트 그 자체
    private GameObject _tile;
    private Material _tileMaterial;

    // 타일 위의 오브젝트
    private GameObject _obstacle;
    private MaterialPropertyBlock _propBlock;

    public bool IsObstacleEnabled { get; set; }

    public void Init()
    {
        _tile = gameObject;
        _propBlock = new MaterialPropertyBlock();
        if (Application.isPlaying)
        {
            // Material 인스턴스화
            // Editor상에서 실행 시 씬에 Material 인스턴스가 영구 저장되는 문제 발생
            _tileMaterial = GetComponent<Renderer>().material; 
        }
        else
        {
            _tileMaterial = GetComponent<Renderer>().sharedMaterial;
        }
        _obstacle = gameObject.transform.GetChild(0).gameObject;
        _obstacle.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0); // 각도를 랜덤으로 하여 랜덤성 부여
        DisableObstacle();
    }

    // 타일 밝기 변경
    // 영웅 시야 범위 내에 있을 시 isVisible = true, 아니면 false
    public void SetBrightness(bool isVisible)
    {
        Color targetColor = isVisible ? Color.white : Color.gray * 0.5f;
        _tileMaterial.DOColor(targetColor, 0.5f); // 타일은 Material Instance를 사용하여 DOTween으로 색상 변경

        var renderers = _obstacle.GetComponentsInChildren<Renderer>();

        // 장애물은 Material Property Block을 사용하여 색상 변경
        foreach (var renderer in renderers)
        {
            renderer.GetPropertyBlock(_propBlock);

            var baseColor = renderer.sharedMaterial.HasProperty("_Color") ?
                renderer.sharedMaterial.GetColor("_Color") : Color.white;

            targetColor = isVisible ? baseColor : baseColor * 0.4f;
            targetColor.a = 1f; // RenderingMode가 Cutout인 Material이 있기 때문에 alpha값은 1로 고정
            _propBlock.SetColor("_Color", targetColor);

            renderer.SetPropertyBlock(_propBlock);
        }
    }

    // 빛의 효과를 받는 타일로 설정
    public void EnableLight()
    {   
        _tile.SetLayerRecursively(LayerMask.NameToLayer("AreaLightTarget"));
    }

    public void EnableObstacle()
    {
        if (!IsObstacleEnabled) return;
        _obstacle.SetActive(true);
    }

    public void DisableObstacle()
    {
        _obstacle.SetActive(false);
    }
}