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

    private HeroInstanceData _heroInstance; // 이 장비 슬롯을 소유한 영웅 인스턴스 데이터
    private int _heroInstanceId => _heroInstance?.InstanceId ?? -1;

    private EquipmentInstanceData _equipmentInstance; // 이 장비 슬롯에 장착된 장비 인스턴스 데이터
    private EquipmentType _equipmentType; // 이 장비 슬롯이 담당하는 장비 타입

    private static InventorySlotDesign _slotDesign = new HeroEquipmentSlotDesign(); // 슬롯 디자인 정보. HeroEquipmentSlotDesign 사용
    private Image _slotImage;

    public override void Init()
    {
        Bind<Image>(typeof(Images));

        _slotImage = GetComponent<Image>();
        _slotImage.sprite = _slotDesign.GetDefaultSlotSprite();
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
            GetImage(Images.Image_Equipment).sprite = _slotDesign.GetDefaultContentSprite();
        }
    }

    public void UnbindEquipment()
    {
        BindEquipment(equipmentInstance: null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _slotImage.sprite = _slotDesign.GetSlotSpriteOnMouseOver();
        if (_equipmentInstance != null)
        {
            ItemDetailUIFactory.CreateItemDetailUI(_equipmentInstance);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _slotImage.sprite = _slotDesign.GetDefaultSlotSprite();
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
