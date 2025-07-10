using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI_CoinToss에서 하나의 Coin을 나타내는 UI
/// </summary>
public class UI_Coin : UI_Base
{
    enum StatImages
    {
        Strength,
        Vitality,
        Dexterity,
        Intelligence,
    }

    enum GameObjects
    {
        Icon_Fail,
        Frame_Success,
    }

    private static readonly Dictionary<StatName, StatImages> _statIconDic = new Dictionary<StatName, StatImages>
    {
        { StatName.Strength, StatImages.Strength },
        { StatName.Vitality, StatImages.Vitality },
        { StatName.Dexterity, StatImages.Dexterity },
        { StatName.Intelligence, StatImages.Intelligence },
    };

    private Image _bg;
    private Image _statIcon;
    private Color _bgOriginalColor;
    private Color _statIconOriginalColor;

    public override void Init() { }

    public void LateInit(StatName statName)
    {
        Bind<Image>(typeof(StatImages));
        Bind<GameObject>(typeof(GameObjects));

        _bg = GetComponent<Image>();
        _bgOriginalColor = _bg.color;
        _statIconOriginalColor = Get<Image>(StatImages.Strength).color;
        
        GetGameObject(GameObjects.Icon_Fail).SetActive(false);
        GetGameObject(GameObjects.Frame_Success).SetActive(false);

        StatImages iconType = _statIconDic[statName];

        _statIcon = Get<Image>(iconType);
        _statIcon.color = _statIconOriginalColor;

        for (int i = 0; i < 4; i++)
            Get<Image>(i).gameObject.SetActive(i == (int)iconType);

        gameObject.SetActive(true);
    }

    /// <summary>
    /// 코인 토스 성공 여부에 따른 색 변화
    /// </summary>
    public void ShowResult(bool isSuccess)
    {
        if (isSuccess)
        {
            _bg.color = Color.green;
            GetGameObject(GameObjects.Frame_Success).SetActive(true);
        }
        else
        {
            _bg.color = Color.gray;
            _statIcon.color = Color.gray;
            GetGameObject(GameObjects.Icon_Fail).SetActive(true);
        }
    }
}
