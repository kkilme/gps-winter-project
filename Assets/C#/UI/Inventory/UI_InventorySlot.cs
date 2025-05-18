using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UI_InventorySlot : UI_Base, IPointerEnterHandler, IPointerExitHandler
{
    enum Texts
    {
        Text_Quantity
    }

    enum Images
    {
        Image_Item
    }

    enum GameObjects
    {
        Detail,
        Quantity
    }

    public ItemData ItemData { get; private set; }
    private int _quantity;
    public int Quantity { 
        get => _quantity; 
        set 
        {
            if (_quantity != 0 && value == 0)
            {
                UnbindItem();
            } else
            {
                _quantityText.text = value.ToString();
            }
            _quantity = value;
        } 
    }
    public bool IsEmpty => ItemData == null;

    private TextMeshProUGUI _quantityText;
    private Image _itemImage;
    private GameObject _detailParent;
    private GameObject _quantityParent;

    private Image _slotImage;
    private static Sprite _slotSprite;
    private static Sprite _slotSpriteOnMouseEnter;

    public override void Init() {}

    public void LateInit()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));

        _quantityText = GetText(Texts.Text_Quantity);
        _itemImage = GetImage(Images.Image_Item);
        _detailParent = GetGameObject(GameObjects.Detail);
        _quantityParent = GetGameObject(GameObjects.Quantity);
        _slotImage = GetComponent<Image>();

        // TODO: 인벤토리별로 Slot의 디자인(Sprite)가 다를 수 있으므로 _slotSpriteOnMouseEnter에 대한 대처가 필요함 ////////////////////////////
        if (_slotSprite == null) _slotSprite = _slotImage.sprite;
        if (_slotSpriteOnMouseEnter == null) _slotSpriteOnMouseEnter = Managers.ResourceMng.Load<Sprite>("Textures/Others/Item_Slot_Selected");
        ////////////////////////////////////////////////////////////////////////////////////////////
        HideDetail();
    }

    /// <summary>
    /// 아이템 데이터와 개수를 UI에 바인딩.
    /// </summary>
    public void BindItem(ItemData itemdata, int quantity)
    {
        ItemData = itemdata;
        Quantity = quantity;
        _itemImage.sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.ITEMIMAGE_PATH_PREFIX + itemdata.ImagePath);
        _detailParent.SetActive(true);
        if (itemdata.ItemType == ItemType.Consumable) // 개수 표시는 소모품에만
        {
            _quantityParent.SetActive(true);
        }
        else
        {
            _quantityParent.SetActive(false);
        }
    }

    public void UnbindItem()
    {
        ItemData = null;
        _quantity = 0;
        HideDetail();
    }

    public void HideDetail()
    {
        _detailParent.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _slotImage.sprite = _slotSpriteOnMouseEnter;
        if (ItemData != null)
        {
            var detailUI = Managers.UIMng.ShowPopupUI<UI_ItemDetail>();
            detailUI.HideInstantly();
            detailUI.ApplyDesign(ItemData);
            detailUI.ShowInstantly();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _slotImage.sprite = _slotSprite;
        Managers.UIMng.ClosePopupUI<UI_ItemDetail>();
    }
}