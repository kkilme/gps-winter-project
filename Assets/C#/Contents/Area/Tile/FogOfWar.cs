using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{   
    // Fade in, out 효과를 주려 했으나, 타일에 장애물이 위치해있다면 장애물도 함께 Fade 효과를 주어야 함
    // 장애물 오브젝트의 경우 Mesh의 Material을 조정해야 하나 구매한 애셋의 매테리얼 특성상 직접 조정하긴 어려워보여 본 기능은 보류
    //[SerializeField]
    //private Sprite _fill;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

}
