using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AreaBaseTile : MonoBehaviour
{
    private GameObject _tile;
    private GameObject _decoration;

    public void Init()
    {
        _tile = gameObject;
        _decoration = gameObject.transform.GetChild(0).gameObject;
        DisableDecoration();
    }

    public void SetLightTargetLayer()
    {   
        _tile.SetLayerRecursively(LayerMask.NameToLayer("AreaLightTarget"));
    }

    public void EnableDecoration()
    {
        _decoration.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360),0);
        _decoration.SetActive(true);
    }

    public void DisableDecoration()
    {
        _decoration.SetActive(false);
    }
}