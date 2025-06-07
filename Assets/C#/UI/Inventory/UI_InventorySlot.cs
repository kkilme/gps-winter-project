using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

/// <summary>
/// 인벤토리에서 한 칸의 슬롯 UI를 담당.
/// </summary>
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
    private Image _contentImage; // 실제 아이템 또는 내용물 이미지
    private GameObject go_detailParent;
    private GameObject go_quantityParent;
    private GameObject go_equippedFlag;

    private Image _slotImage; // 슬롯의 배경 이미지
    private static Sprite sp_slotImage;
    private static Sprite sp_slotImageOnMouseEnter;

    public override void Init() { }

    public void LateInit(string slotSpriteOnMouseEnterPath, Action<UI_InventorySlot> onClickAction, Sprite defaultContentSprite = null, bool isCustomSlot = false)
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));

        _quantityText = GetText(Texts.Text_Quantity);
        _contentImage = GetImage(Images.Image_Item);
        go_detailParent = GetGameObject(GameObjects.Detail);
        go_quantityParent = GetGameObject(GameObjects.Quantity);
        go_equippedFlag = GetGameObject(GameObjects.Flag_Equipped);
        _slotImage = GetComponent<Image>();

        if (sp_slotImage == null) sp_slotImage = _slotImage.sprite;
        if (sp_slotImageOnMouseEnter == null) sp_slotImageOnMouseEnter = Managers.ResourceMng.Load<Sprite>(slotSpriteOnMouseEnterPath);
        OnClickAction -= onClickAction;
        OnClickAction += onClickAction;
        _isCustomSlot = isCustomSlot;

        AdjustQuantityRectSize();
        HideDetail();

        if (defaultContentSprite != null) ShowContentImageOnly(); _contentImage.sprite = defaultContentSprite;
    }

    /// <summary>
    /// Quantity/equippedFlag RectTransform 크기 조정.
    /// </summary>
    private void AdjustQuantityRectSize()
    {
        RectTransform qualityRect = go_quantityParent.GetComponent<RectTransform>();
        RectTransform equippedFlagRect = go_equippedFlag.GetComponent<RectTransform>();

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

        _contentImage.sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.ITEMIMAGE_PATH_PREFIX + ItemData.ImagePath);

        go_detailParent.SetActive(true);
        if (ItemInstanceData.ItemType == ItemType.Consumable) // 개수 표시는 현재 소모품에만
        {
            go_quantityParent.SetActive(true);
        }
        else
        {
            go_quantityParent.SetActive(false);
        }

        if(itemInstanceData is EquipmentInstanceData equipmentInstanceData)
        {
            go_equippedFlag.SetActive(equipmentInstanceData.IsEquipped);
        }
        else
        {
            go_equippedFlag.SetActive(false);
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
        go_detailParent.SetActive(true);
        go_quantityParent.SetActive(false);
        go_equippedFlag.SetActive(false);
    }

    private void HideDetail()
    {
        go_detailParent.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (sp_slotImageOnMouseEnter) _slotImage.sprite = sp_slotImageOnMouseEnter;
        if (ItemInstanceData != null)
        {
            ItemDetailUIFactory.CreateItemDetailUI(ItemInstanceData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _slotImage.sprite = sp_slotImage;
        Managers.UIMng.ClosePopupUI<UI_ItemDetailPopup>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Managers.UIMng.ClosePopupUI<UI_ItemDetailPopup>();
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnClickAction?.Invoke(this);
        }
    }
}