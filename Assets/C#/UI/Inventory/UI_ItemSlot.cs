using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 인벤토리에서 한 칸의 슬롯 UI를 담당.
/// </summary>
public class UI_ItemSlot : UI_Base, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    enum Texts
    {
        Text_Quantity
    }

    enum Images
    {
        Image_Content
    }

    enum GameObjects
    {
        Image_Content,
        Quantity,
        Flag_Equipped
    }

    public ItemInstanceData ItemInstanceData { get; private set; }
    public ItemData ItemData { get; private set; }

    private ItemSlotDesign _slotDesign; // 슬롯 디자인 정보

    private int _quantity;
    private TextMeshProUGUI _quantityText;
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity != 0 && value == 0)
            {
                UnbindItem();
            }
            else if (value > 1)
            {
                go_quantity.SetActive(true);
                _quantityText.text = value.ToString();
            }
            else
            {
                go_quantity.SetActive(false);
            }
            _quantity = value;
        }
    }

    /// <summary>
    /// 새 아이템이 들어갈 수 있는 슬롯인지 여부.
    /// </summary>
    public bool IsEmpty => ItemInstanceData == null && ItemData == null && !_isCustomSlot;
    private bool _isCustomSlot;

    public Action<UI_ItemSlot> OnClickAction { get; set; }

    private GameObject go_imageObject;
    private GameObject go_quantity;
    private GameObject go_equippedFlag;

    private Image _contentImage; // 실제 아이템 또는 내용물 이미지
    private Image _slotImage; // 슬롯의 배경 이미지

    public override void Init() { }

    public void LateInit(ItemSlotDesign slotDesign, bool isCustomSlot = false)
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));

        go_imageObject = GetGameObject(GameObjects.Image_Content);
        go_quantity = GetGameObject(GameObjects.Quantity);
        go_equippedFlag = GetGameObject(GameObjects.Flag_Equipped);
        _slotDesign = slotDesign;
        _isCustomSlot = isCustomSlot;

        AdjustQuantityRectSize();
        HideDetails();

        _quantityText = GetText(Texts.Text_Quantity);

        _slotImage = GetComponent<Image>();
        _slotImage.sprite = slotDesign.GetDefaultSlotSprite();

        _contentImage = GetImage(Images.Image_Content);
        SetContentImage(slotDesign.GetDefaultContentSprite());
    }

    /// <summary>
    /// Quantity/equippedFlag RectTransform 크기 조정.
    /// </summary>
    private void AdjustQuantityRectSize()
    {
        RectTransform qualityRect = go_quantity.GetComponent<RectTransform>();
        RectTransform equippedFlagRect = go_equippedFlag.GetComponent<RectTransform>();

        float width, height;

        GridLayoutGroup gridLayout = transform.parent.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
        {
            RectTransform rect = GetComponent<RectTransform>();
            width = rect.rect.width;
            height = rect.rect.height;
        }
        else
        {
            width = gridLayout.cellSize.x;
            height = gridLayout.cellSize.y;
        }
        qualityRect.sizeDelta = new Vector2(width / 3, height / 3);
        equippedFlagRect.sizeDelta = new Vector2(width / 3, height / 3);
    }

    /// <summary>
    /// ItemInstanceData와 개수를 UI에 바인딩.
    /// </summary>
    public void BindItem(ItemInstanceData itemInstanceData, int quantity = 1)
    {
        ItemInstanceData = itemInstanceData;
        ItemData = itemInstanceData.ItemData;
        Quantity = quantity;

        SetContentImage(ItemData);

        if (itemInstanceData is EquipmentInstanceData equipmentInstanceData)
        {
            go_equippedFlag.SetActive(equipmentInstanceData.IsEquipped);
        }
        else
        {
            go_equippedFlag.SetActive(false);
        }
    }

    /// <summary>
    /// ItemData와 개수를 UI에 바인딩.
    /// </summary>
    public void BindItem(ItemData itemData, int quantity = 1)
    {
        ItemData = itemData;
        Quantity = quantity;
        SetContentImage(itemData);
        go_equippedFlag.SetActive(false);
    }

    /// <summary>
    /// 바인딩된 ItemInstanceData 및 ItemData 해제.
    /// </summary>
    public void UnbindItem()
    {
        ItemInstanceData = null;
        ItemData = null;
        _quantity = 0;
        HideDetails();
        SetContentImage(_slotDesign.GetDefaultContentSprite());
    }

    /// <summary>
    /// itemData에 해당하는 이미지를 Content 이미지로 표시.
    /// </summary>
    private void SetContentImage(ItemData itemData)
    {
        Sprite sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.ITEMIMAGE_PATH_PREFIX + itemData.ImagePath);
        SetContentImage(sprite);
    }

    /// <summary>
    /// 슬롯 Content 이미지의 스프라이트를 sprite로 설정.
    /// </summary>
    private void SetContentImage(Sprite sprite)
    {
        if (sprite == null) return;

        go_imageObject.SetActive(true);
        _contentImage.sprite = sprite;
    }

    private void HideDetails()
    {
        go_imageObject.SetActive(false);
        go_quantity.SetActive(false);
        go_equippedFlag.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스 진입 시 slotDesign에 따른 UI 변화
        if (_slotDesign != null)
        {
            Sprite mouseOverSprite = _slotDesign.GetSlotSpriteOnMouseOver();
            if (mouseOverSprite) _slotImage.sprite = mouseOverSprite;
        }

        // 바인딩된 아이템의 상세 정보 UI 생성
        if (ItemInstanceData != null)
        {
            ItemDetailUIController.ShowItemDetailUI(ItemInstanceData);
        }
        else if (ItemData != null)
        {
            ItemDetailUIController.ShowItemDetailUI(ItemData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_slotDesign != null) _slotImage.sprite = _slotDesign.GetDefaultSlotSprite();
        ItemDetailUIController.HideItemDetailUI();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ItemDetailUIController.HideItemDetailUI();
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnClickAction?.Invoke(this);
        }
    }

    public void OnDestroy()
    {
        OnClickAction = null;
    }
}