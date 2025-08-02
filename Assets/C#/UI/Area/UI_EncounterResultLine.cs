using TMPro;
using UnityEngine;


public class UI_EncounterResultLine : UI_Base
{
    enum Texts
    {
        Text_CoinCount,
        Text_Percent,
        Text_ResultDescription
    }

    enum GameObjects
    {
        Highlight,
    }


    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));
        GetGameObject(GameObjects.Highlight).SetActive(false);
    }

    public void SetData(string coinCount, int percent, string description)
    {
        Get<TextMeshProUGUI>(Texts.Text_CoinCount).text = coinCount;
        Get<TextMeshProUGUI>(Texts.Text_Percent).text = $"({percent}%)";
        Get<TextMeshProUGUI>(Texts.Text_ResultDescription).text = description;
    }

    public void Highlight()
    {
        GetGameObject(GameObjects.Highlight).SetActive(true);
    }
}
