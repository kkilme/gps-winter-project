using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_HeroProfile : UI_CreatureProfile
{
    enum HeroTexts
    {
        Text_Flee
    }

    public override void Init()
    {
        base.Init();
        Bind<TextMeshProUGUI>(typeof(HeroTexts));
        GetText(Enum.GetNames(typeof(Texts)).Length + HeroTexts.Text_Flee).gameObject.SetActive(false);
    }

    public void OnFlee()
    {
        _canvasGroup.DOFade(0.33f, 1f).OnComplete(() => GetText(Enum.GetNames(typeof(Texts)).Length + HeroTexts.Text_Flee).gameObject.SetActive(true));
    }
}
