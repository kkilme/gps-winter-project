using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 테스트용 아이템 클래스
public class TestItem
{
    public Sprite Icon;
    public string Name, Description;
    public int Price;

    public void PurchaseItem()
    {
        Debug.Log("Purchase!");
    }

    public void SellItem()
    {
        Debug.Log("Sell!");
    }
}

public class UI_PurchaseBoard : UI_Base
{ 
    enum GameObjects
    {
        Content,
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        List<TestItem> testItems = new List<TestItem>();

        //////////////////////////////////////////
        // Test Code
        TestItem testItem = new TestItem()
        {
            Name = "Lorem Ipsum",
            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
            "Ut vehicula ante ac nunc fringilla elementum. Morbi sodales",
            Price = 10,
        };

        for (int i = 0; i < 10; i++)
            testItems.Add(testItem);
        //////////////////////////////////////////


        foreach (var item in testItems)
        {
            UI_ItemContent itemContent = Managers.UIMng.MakeSubItemUI<UI_ItemContent>(GetGameObject(GameObjects.Content).transform);
            itemContent.Init(item);

            void PurchaseItem(PointerEventData eventData)
            {
                item.PurchaseItem();
            }

            void ShowItemInfo(PointerEventData eventData)
            {
                UI_ItemInfo itemInfo = Managers.UIMng.ShowPopupUI<UI_ItemInfo>();
                itemInfo.Init(item);
                itemContent.gameObject.BindEvent((PointerEventData) =>
                {
                    itemInfo.RectTransform.position = Input.mousePosition;
                }, Define.UIEvent.Stay);
                itemContent.gameObject.BindEvent((PointerEventData) => Managers.UIMng.ClosePopupUI(itemInfo), Define.UIEvent.Exit);
            }

            itemContent.gameObject.BindEvent(PurchaseItem, Define.UIEvent.DoubleClick);
            itemContent.gameObject.BindEvent(ShowItemInfo, Define.UIEvent.Enter);
        }
    }

    private void OnEnable()
    {
        this.GetComponent<RectTransform>().DOLocalMoveX(500, 0.5f).From(true).SetEase(Ease.OutCirc).SetDelay(0.05f);
    }
}
