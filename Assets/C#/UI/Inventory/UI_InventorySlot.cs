using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class UI_InventorySlot : UI_Base, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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

    public ItemInstanceData ItemInstanceData { get; private set; }
    public ItemData ItemData => ItemInstanceData?.ItemData;
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

    public Action<UI_InventorySlot> OnClickAction;

    private TextMeshProUGUI _quantityText;
    private Image _itemImage;
    private GameObject _detailParent;
    private GameObject _quantityParent;

    private Image _slotImage;
    private static Sprite _slotSprite;
    private static Sprite _slotSpriteOnMouseEnter;

    public override void Init() { }

    public void LateInit(string slotSpriteOnMouseEnterPath, Action<UI_InventorySlot> onClickAction)
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));

        _quantityText = GetText(Texts.Text_Quantity);
        _itemImage = GetImage(Images.Image_Item);
        _detailParent = GetGameObject(GameObjects.Detail);
        _quantityParent = GetGameObject(GameObjects.Quantity);
        _slotImage = GetComponent<Image>();

        if (_slotSprite == null) _slotSprite = _slotImage.sprite;
        if (_slotSpriteOnMouseEnter == null) _slotSpriteOnMouseEnter = Managers.ResourceMng.Load<Sprite>(slotSpriteOnMouseEnterPath);
        OnClickAction = onClickAction;

        AdjustQuantityRectSize();
        HideDetail();
    }

    /// <summary>
    /// Quantity RectTransform 크기 조정.
    /// </summary>
    private void AdjustQuantityRectSize()
    {
        RectTransform qualityRect = _quantityParent.GetComponent<RectTransform>();
        RectTransform parentRect = GetComponent<RectTransform>();
        qualityRect.sizeDelta = new Vector2(parentRect.sizeDelta.x / 3, parentRect.sizeDelta.y / 3);
    }

    /// <summary>
    /// 아이템 데이터와 개수를 UI에 바인딩.
    /// </summary>
    public void BindItem(ItemInstanceData itemInstanceData, int quantity)
    {
        ItemInstanceData = itemInstanceData;
        Quantity = quantity;

        //_itemImage.GetComponent<RectTransform>().sizeDelta = new Vector2(_rect.sizeDelta.x - 8, _rect.sizeDelta.y - 8);
        _itemImage.sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.ITEMIMAGE_PATH_PREFIX + ItemData.ImagePath);

        _detailParent.SetActive(true);
        if (ItemInstanceData.ItemType == ItemType.Consumable) // 개수 표시는 소모품에만
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
        ItemInstanceData = null;
        _quantity = 0;
        HideDetail();
    }

    public void HideDetail()
    {
        _detailParent.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_slotSpriteOnMouseEnter) _slotImage.sprite = _slotSpriteOnMouseEnter;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        Managers.UIMng.ClosePopupUI<UI_ItemDetail>();
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnClickAction?.Invoke(this);
        }
    }
}