using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Bag : UI_Base
{
    enum Items
    {
        First,
        Second,
        Third,
        Fourth,
        Fifth,
        Sixth
    }
    
    public override void Init()
    {
        Bind<UnityEngine.UI.Image>(typeof(Items));
    }
    
    public void BindBag(Bag bag)
    {
        bag.ContentChange += ShowItems;
    }


    private void ShowItems(Bag bag)
    {
        for (int i = 0; i < 6; i++)
        {
            var currentItem = bag.Items[i];
            var item = GetImage((Items)i).transform.GetChild(0).gameObject;
            var itemIcon = item.GetComponent<UnityEngine.UI.Image>();
            var itemDetail = item.transform.GetChild(0).GetComponent<UI_ItemDescription>();

            if (currentItem != null)
            {
                // TODO - item Image 넣기
                //itemUI.image = currentItem.image;
                itemIcon.enabled = true;
                
                itemDetail.SetInfo(currentItem);
                item.BindEvent(itemDetail.OnMouseEnter, Define.UIEvent.Enter);
                item.BindEvent(itemDetail.OnMouseExit, Define.UIEvent.Exit);
            }
            else
            {
                itemIcon.enabled = false;
                item.ClearEvent();
            }

            itemDetail.ChangeImageVisibility(false);
        }
    }
}
