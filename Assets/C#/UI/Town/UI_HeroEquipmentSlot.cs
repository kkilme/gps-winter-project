using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_HeroEquipmentSlot : UI_Base, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    enum Images
    {
        Image_Equipment,
    }

    private int _heroInstanceId;
    private int _equipmentDataId;
    private EquipmentType _equipmentType;

    private bool _isSelectingEquipment = false;

    private static Sprite _defaultSprite; // 빈 장비 슬롯에 들어갈 기본 스프라이트
    private Image _slotImage;
    private static Sprite _slotSprite;
    private static Sprite _slotSpriteOnMouseEnter;

    public override void Init()
    {
        _defaultSprite = Managers.ResourceMng.Load<Sprite>("Textures/Others/PictoIcon_Plus");
        Bind<Image>(typeof(Images));

        _slotImage = GetComponent<Image>();
        if (_slotSprite == null) _slotSprite = _slotImage.sprite;
        if (_slotSpriteOnMouseEnter == null) 
            _slotSpriteOnMouseEnter = Managers.ResourceMng.Load<Sprite>("Textures/Others/ItemSlot_Selected");
    }

    public void LateInit(int heroInstanceId, EquipmentType type)
    {
        _heroInstanceId = heroInstanceId;
        _equipmentType = type;

        BindEquipment(Managers.HeroMng.HeroStorage.GetEquippedEquipment(_heroInstanceId, _equipmentType));
    }

    public void BindEquipment(int equipmentDataId)
    {
        _equipmentDataId = equipmentDataId;
        if (Managers.DataMng.EquipmentDataDict.TryGetValue(equipmentDataId, out EquipmentData equipmentData))
        {
            GetImage(Images.Image_Equipment).sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.ITEMIMAGE_PATH_PREFIX + equipmentData.ImagePath);
        }
        else
        {
            GetImage(Images.Image_Equipment).sprite = _defaultSprite;
        }
    }

    public void UnbindEquipment()
    {
        _equipmentDataId = -1;
        GetImage(Images.Image_Equipment).sprite = _defaultSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_slotSpriteOnMouseEnter) _slotImage.sprite = _slotSpriteOnMouseEnter;
        if (_equipmentDataId != -1 && !_isSelectingEquipment)
        {
            UI_ItemDetail itemDetail = Managers.UIMng.ShowPopupUI<UI_ItemDetail>();
            itemDetail.HideInstantly();
            itemDetail.ApplyDesign(Managers.DataMng.EquipmentDataDict[_equipmentDataId]);
            itemDetail.ShowInstantly();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_slotSpriteOnMouseEnter) _slotImage.sprite = _slotSprite;
        Managers.UIMng.ClosePopupUI<UI_ItemDetail>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _isSelectingEquipment = true;
        Managers.UIMng.ClosePopupUI<UI_EquipmentSelectWindow>();
        UI_EquipmentSelectWindow equipmentSelectWindow = Managers.UIMng.ShowPopupUI<UI_EquipmentSelectWindow>();
        equipmentSelectWindow.LateInit(this, _equipmentType);

        Managers.UIMng.ClosePopupUI<UI_ItemDetail>();
    }

    public void OnEquipmentSelected(UI_InventorySlot selectedSlot)
    {
        _isSelectingEquipment = false;
        if (selectedSlot == null || selectedSlot.IsEmpty)
        {
            return;
        }
        int equipmentDataId = selectedSlot.ItemData.DataId;
        if (equipmentDataId == -1) // 선택 취소
        {
            UnbindEquipment();
            Managers.HeroMng.HeroStorage.UnEquipEquipment(_heroInstanceId, _equipmentType);
        }
        else // 장비 선택
        {
            BindEquipment(equipmentDataId);
            Managers.HeroMng.HeroStorage.EquipEquipment(_heroInstanceId, _equipmentType, equipmentDataId);
        }
    }
}
