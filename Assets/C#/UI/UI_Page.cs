using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_Page : UI_Base
{
    private void OnEnable()
    {
        this.GetComponent<RectTransform>().DOLocalMoveX(-250, 0.5f).From(true).SetEase(Ease.OutCirc);
    }
}
