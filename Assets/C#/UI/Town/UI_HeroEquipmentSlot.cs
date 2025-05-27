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

    private HeroInstanceData _heroInstance;
    private int _heroInstanceId => _heroInstance?.InstanceId ?? -1;
    private EquipmentInstanceData _equipmentInstance;
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

    public void LateInit(HeroInstanceData heroInstance, EquipmentType type)
    {
        _heroInstance = heroInstance;
        _equipmentType = type;
        _heroInstance.OnEquipmentChanged -= BindEquipment;
        _heroInstance.OnEquipmentChanged += BindEquipment; // 영웅의 장비가 변경될 때마다 BindEquipment 호출

        BindEquipment(Managers.HeroMng.HeroStorage.GetEquippedEquipment(_heroInstanceId, _equipmentType));
    }

    public void BindEquipment(HeroInstanceData heroInstanceData)
    {
        if (_heroInstance != null && Managers.HeroMng.HeroStorage.GetEquippedEquipment(_heroInstanceId, _equipmentType) is EquipmentInstanceData equipmentInstance)
        {
            BindEquipment(equipmentInstance);
        }
        else
        {
            UnbindEquipment();
        }
    }

    public void BindEquipment(EquipmentInstanceData equipmentInstance)
    {
        _equipmentInstance = equipmentInstance;
        if (equipmentInstance != null && Managers.DataMng.EquipmentDataDict.TryGetValue(equipmentInstance.ItemDataId, out EquipmentData equipmentData))
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
        _equipmentInstance = null;
        GetImage(Images.Image_Equipment).sprite = _defaultSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_slotSpriteOnMouseEnter) _slotImage.sprite = _slotSpriteOnMouseEnter;
        if (_equipmentInstance != null && !_isSelectingEquipment)
        {
            UI_ItemDetail itemDetail = Managers.UIMng.ShowPopupUI<UI_ItemDetail>();
            itemDetail.HideInstantly();
            itemDetail.ApplyDesign(Managers.DataMng.EquipmentDataDict[_equipmentInstance.ItemDataId]);
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
            UnbindEquipment();
            Managers.HeroMng.HeroStorage.UnEquipEquipment(_heroInstanceId, _equipmentType);
            return;
        }

        EquipmentInstanceData selectedEquipment = selectedSlot.ItemInstanceData as EquipmentInstanceData;
        BindEquipment(selectedEquipment);
        Managers.HeroMng.HeroStorage.EquipEquipment(_heroInstanceId, _equipmentType, selectedEquipment.InstanceId);
    }

    private void OnDestroy()
    {
        if(_heroInstance != null) _heroInstance.OnEquipmentChanged -= BindEquipment;
    }
}
