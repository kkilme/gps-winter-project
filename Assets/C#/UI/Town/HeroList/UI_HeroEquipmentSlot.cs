using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// HeroDetail UI에서 영웅의 장비 슬롯 하나를 담당하는 클래스.
/// </summary>
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

    private static Sprite _defaultSprite; // 빈 장비 슬롯에 들어갈 기본 스프라이트
    private Image _slotImage;
    private static Sprite _slotSprite;
    private static Sprite _slotSpriteOnMouseEnter;

    public override void Init()
    {
        _defaultSprite = Managers.ResourceMng.Load<Sprite>("Textures/PictoIcons/PictoIcon_Plus");
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

    /// <summary>
    /// 장비 슬롯에 heroInstanceData의 _equipmentType에 해당하는 장비 바인딩
    /// </summary>
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

    /// <summary>
    /// 장비 슬롯에 equipmentInstanceData 바인딩
    /// </summary>
    /// <param name="equipmentInstance"></param>
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
        if (_equipmentInstance != null)
        {
            ItemDetailUIFactory.CreateItemDetailUI(_equipmentInstance);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_slotSpriteOnMouseEnter) _slotImage.sprite = _slotSprite;
        Managers.UIMng.ClosePopupUI<UI_ItemDetailPopup>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 이미 열린 장비 선택 창이 있다면 닫기
        Managers.UIMng.ClosePopupUI<UI_EquipmentSelectPopup>();

        // 장비 선택 창 띄우기
        UI_EquipmentSelectPopup equipmentSelectWindow = Managers.UIMng.ShowPopupUI<UI_EquipmentSelectPopup>();
        equipmentSelectWindow.LateInit(this, _equipmentType);

        Managers.UIMng.ClosePopupUI<UI_ItemDetailPopup>();
    }

    // 장비 선택 창에서 장비를 선택하면 호출됨
    public void ChangeEquipment(EquipmentInstanceData equipment)
    {
        // 장비 해제
        if (equipment == null)
        {
            UnbindEquipment();
            Managers.HeroMng.HeroStorage.UnEquipEquipment(_heroInstanceId, _equipmentType);
            return;
        }

        BindEquipment(equipment);
        Managers.HeroMng.HeroStorage.EquipEquipment(_heroInstanceId, _equipmentType, equipment.InstanceId);
    }

    private void OnDestroy()
    {
        if(_heroInstance != null) _heroInstance.OnEquipmentChanged -= BindEquipment;
    }
}
