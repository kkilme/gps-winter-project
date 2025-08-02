using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI_CoinToss에서 하나의 Coin을 나타내는 UI
/// </summary>
public class UI_Coin : UI_Base
{
    enum StatImage
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

    private static readonly Dictionary<StatName, StatImage> _statIconDic = new Dictionary<StatName, StatImage>
    {
        { StatName.Strength, StatImage.Strength },
        { StatName.Vitality, StatImage.Vitality },
        { StatName.Dexterity, StatImage.Dexterity },
        { StatName.Intelligence, StatImage.Intelligence },
    };

    private Image _statImage;

    private Color _statImageOriginalColor;

    public override void Init() { }

    public void LateInit()
    {
        Bind<Image>(typeof(StatImage));
        Bind<GameObject>(typeof(GameObjects));

        _statImageOriginalColor = Get<Image>(StatImage.Strength).color;

        GetGameObject(GameObjects.Icon_Fail).SetActive(false);
        GetGameObject(GameObjects.Frame_Success).SetActive(false);
    }

    public void Show(StatName statName)
    {
        GetGameObject(GameObjects.Icon_Fail).SetActive(false);
        GetGameObject(GameObjects.Frame_Success).SetActive(false);

        StatImage statImage = _statIconDic[statName];

        _statImage = Get<Image>(statImage);
        _statImage.color = _statImageOriginalColor;

        for (int i = 0; i < 4; i++)
            Get<Image>(i).gameObject.SetActive(i == (int)statImage);

        gameObject.SetActive(true);
    }

    /// <summary>
    /// 코인 토스 성공 여부에 따른 색 변화
    /// </summary>
    public void ShowResult(bool isSuccess)
    {
        if (isSuccess)
        {
            GetGameObject(GameObjects.Frame_Success).SetActive(true);
        }
        else
        {
            _statImage.color = Color.gray;
            GetGameObject(GameObjects.Icon_Fail).SetActive(true);
        }
    }
}
