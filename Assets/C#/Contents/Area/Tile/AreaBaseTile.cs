using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

/// <summary>
/// Area 맵의 육각형 타일 게임오브젝트에 붙는 스크립트
/// </summary>
public class AreaBaseTile : MonoBehaviour
{   
    // 플레이어가 밟고 서는 타일 오브젝트 그 자체
    private GameObject _tile;
    private Material _tileMaterial;

    // 타일 위의 오브젝트
    private GameObject _obstacle;

    public bool IsObstacleGenerated { get; set; } // 타일 위의 장애물 활성화 여부. 장애물 게임오브젝트의 active/inactive 상태가 아니라 맵 생성 단계에서 이 타일에 장애물을 생성하였는지 여부를 나타냄.

    public void Init()
    {
        _tile = gameObject;
        _tileMaterial = Application.isPlaying ? GetComponent<Renderer>().material : GetComponent<Renderer>().sharedMaterial; // Material 인스턴스화. Editor상에서 인스턴스화 시 치명적이기 때문에 따로 처리. 

        _obstacle = gameObject.transform.GetChild(0).gameObject;
        _obstacle.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0); // 각도를 랜덤으로 하여 랜덤성 부여

        DisableObstacle();
    }

    /// <summary>
    /// 타일 & 장애물 밝기 변경
    /// </summary>
    /// <param name="isVisible">영웅 시야 범위 내에 있을 시 true, 아니면 false</param>
    public void SetBrightness(bool isVisible)
    {
        Color targetColor = isVisible ? Color.white : Color.gray * 0.5f;
        _tileMaterial.DOColor(targetColor, 0.5f); // 타일은 Material Instance를 사용하여 DOTween으로 색상 변경

        var renderers = _obstacle.GetComponentsInChildren<Renderer>();
        RenderUtility.SetRenderersBrightness(renderers, isVisible ? 1f : 0.4f); // 장애물은 Material Property Block을 사용하여 색상 변경
    }

    /// <summary>
    /// 빛의 효과를 받는 타일로 설정
    /// </summary>
    public void EnableLight()
    {   
        _tile.SetLayerRecursively(LayerMask.NameToLayer("AreaLightTarget"));
    }

    public void OnCollapse()
    {
        IsObstacleGenerated = false;
        SetBrightness(false);
        DisableObstacle();
    }

    public void EnableObstacle()
    {
        if (!IsObstacleGenerated) return;
        _obstacle.SetActive(true);
    }

    public void DisableObstacle()
    {
        _obstacle.SetActive(false);
    }
}