using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// UI_InventorySlot과 달리, 단순히 아이템 하나를 바인딩하여 표시하는 UI. ItemInstanceData가 아닌 ItemData를 바인딩한다.
/// </summary>
public class UI_ItemSlot : UI_Base, IPointerEnterHandler, IPointerExitHandler
{
    enum Images
    {
        Image_Item
    }

    private ItemData _itemData;

    public override void Init()
    {
        Bind<Image>(typeof(Images));
        _itemData = null;
        GetImage(Images.Image_Item).gameObject.SetActive(false);
    }

    /// <summary>
    /// ItemData 바인딩 및 해당 아이템의 이미지 표시.
    /// </summary>
    public void BindItem(ItemData itemData)
    {
        _itemData = itemData;
        if (_itemData != null)
        {
            GetImage(Images.Image_Item).gameObject.SetActive(true);
            GetImage(Images.Image_Item).sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.ITEMIMAGE_PATH_PREFIX + _itemData.ImagePath);
        }
        else
        {
            GetImage(Images.Image_Item).gameObject.SetActive(false);
        }
    }
    
    public void UnbindItem()
    {
        _itemData = null;
        GetImage(Images.Image_Item).gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_itemData != null)
        {
            ItemDetailUIFactory.CreateItemDetailUI(_itemData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Managers.UIMng.ClosePopupUI<UI_ItemDetailPopup>();
    }
}
