using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_TestInven_TestItem : UI_Base
{
    // 종류가 적을땐 GameObjects로 통일
    enum GameObjects
    {
        ItemIcon,
        ItemNameText
    }

    private string _itemName;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        GetGameObject(GameObjects.ItemNameText).GetComponent<TextMeshProUGUI>().text = _itemName;
        
        GetGameObject(GameObjects.ItemIcon).gameObject.BindEvent(OnButtonClicked, Define.UIEvent.Click);
    }

    public void SetInfo(string itemName)
    {
        this._itemName = itemName;
    }

    public void OnButtonClicked(PointerEventData data)
    {
        Debug.Log($"{_itemName} Click!");
    }
}
