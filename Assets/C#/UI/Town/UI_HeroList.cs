using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_HeroList : UI_Base
{
    enum GameObjects
    {

    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
    }
}
