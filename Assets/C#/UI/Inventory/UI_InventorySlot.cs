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
        Quantity,
        Flag_Equipped
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
    public bool IsEmpty => ItemInstanceData == null && !_isCustomSlot;
    private bool _isCustomSlot;

    public Action<UI_InventorySlot> OnClickAction;

    private TextMeshProUGUI _quantityText;
    private Image _contentImage;
    private GameObject _detailParent;
    private GameObject _quantityParent;
    private GameObject _equippedFlag;

    private Image _slotDesign;
    private static Sprite _slotDesignSprite;
    private static Sprite _slotDesignSpriteOnMouseEnter;

    public override void Init() { }

    public void LateInit(string slotSpriteOnMouseEnterPath, Action<UI_InventorySlot> onClickAction, Sprite defaultSprite = null, bool isCustomSlot = false)
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));

        _quantityText = GetText(Texts.Text_Quantity);
        _contentImage = GetImage(Images.Image_Item);
        _detailParent = GetGameObject(GameObjects.Detail);
        _quantityParent = GetGameObject(GameObjects.Quantity);
        _equippedFlag = GetGameObject(GameObjects.Flag_Equipped);
        _slotDesign = GetComponent<Image>();

        if (_slotDesignSprite == null) _slotDesignSprite = _slotDesign.sprite;
        if (_slotDesignSpriteOnMouseEnter == null) _slotDesignSpriteOnMouseEnter = Managers.ResourceMng.Load<Sprite>(slotSpriteOnMouseEnterPath);
        OnClickAction -= onClickAction;
        OnClickAction += onClickAction;
        _isCustomSlot = isCustomSlot;

        AdjustQuantityRectSize();
        HideDetail();

        if (defaultSprite != null) ShowContentImageOnly(); _contentImage.sprite = defaultSprite;
    }

    /// <summary>
    /// Quantity/equippedFlag RectTransform 크기 조정.
    /// </summary>
    private void AdjustQuantityRectSize()
    {
        RectTransform qualityRect = _quantityParent.GetComponent<RectTransform>();
        RectTransform equippedFlagRect = _equippedFlag.GetComponent<RectTransform>();

        GridLayoutGroup gridLayout = GetComponentInParent<GridLayoutGroup>();
        float cellWidth = gridLayout.cellSize.x;
        float cellHeight = gridLayout.cellSize.y;
        qualityRect.sizeDelta = new Vector2(cellWidth / 3, cellHeight / 3);
        equippedFlagRect.sizeDelta = new Vector2(cellWidth / 3, cellHeight / 3);
    }

    /// <summary>
    /// 아이템 데이터와 개수를 UI에 바인딩.
    /// </summary>
    public void BindItem(ItemInstanceData itemInstanceData, int quantity)
    {
        ItemInstanceData = itemInstanceData;
        Quantity = quantity;

        //_itemImage.GetComponent<RectTransform>().sizeDelta = new Vector2(_rect.sizeDelta.x - 8, _rect.sizeDelta.y - 8);
        _contentImage.sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.ITEMIMAGE_PATH_PREFIX + ItemData.ImagePath);

        _detailParent.SetActive(true);
        if (ItemInstanceData.ItemType == ItemType.Consumable) // 개수 표시는 소모품에만
        {
            _quantityParent.SetActive(true);
        }
        else
        {
            _quantityParent.SetActive(false);
        }

        if(itemInstanceData is EquipmentInstanceData equipmentInstanceData)
        {
            _equippedFlag.SetActive(equipmentInstanceData.IsEquipped);
        }
        else
        {
            _equippedFlag.SetActive(false);
        }
    }

    public void UnbindItem()
    {
        ItemInstanceData = null;
        _quantity = 0;
        HideDetail();
    }

    private void ShowContentImageOnly()
    {
        _detailParent.SetActive(true);
        _quantityParent.SetActive(false);
        _equippedFlag.SetActive(false);
    }

    private void HideDetail()
    {
        _detailParent.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_slotDesignSpriteOnMouseEnter) _slotDesign.sprite = _slotDesignSpriteOnMouseEnter;
        if (ItemInstanceData != null)
        {
            var detailUI = Managers.UIMng.ShowPopupUI<UI_ItemDetail>();
            detailUI.HideInstantly();
            detailUI.ApplyDesign(ItemInstanceData);
            detailUI.ShowInstantly();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _slotDesign.sprite = _slotDesignSprite;
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