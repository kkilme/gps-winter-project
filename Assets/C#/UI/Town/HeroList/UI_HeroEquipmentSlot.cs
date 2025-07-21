using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
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

    private ItemSlotDesign _slotDesign; // 슬롯 디자인 정보. PlusIconItemSlotDesign 사용
    private Image _slotImage;

    private bool _enableEquipmentChange = true; // 장비 변경 가능 여부. 기본값은 true.

    public override void Init()
    {
        Bind<Image>(typeof(Images));
    }

    public void LateInit(HeroInstanceData heroInstance, EquipmentType type, ItemSlotDesign slotDesign, bool enableEquipmentChange = true)
    {
        _heroInstance = heroInstance;
        _equipmentType = type;
        _slotDesign = slotDesign;
        _enableEquipmentChange = enableEquipmentChange;

        _slotImage = GetComponent<Image>();
        _slotImage.sprite = slotDesign.GetDefaultSlotSprite();
        
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
    public void BindEquipment(EquipmentInstanceData equipmentInstance)
    {
        _equipmentInstance = equipmentInstance;
        if (equipmentInstance != null && Managers.DataMng.EquipmentDataDict.TryGetValue(equipmentInstance.ItemDataId, out EquipmentData equipmentData))
        {
            Image contentImage = GetImage(Images.Image_Equipment);
            contentImage.gameObject.SetActive(true);
            contentImage.sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.ITEMIMAGE_PATH_PREFIX + equipmentData.ImagePath);
        }
        else
        {
            Image contentImage = GetImage(Images.Image_Equipment);
            Sprite defaultSprite = _slotDesign.GetDefaultContentSprite();
            if(defaultSprite != null)
            {
                contentImage.gameObject.SetActive(true);
                contentImage.sprite = defaultSprite;
            }
            else
            {
                contentImage.gameObject.SetActive(false);
            }
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
        if(!_enableEquipmentChange) return; // 장비 변경이 비활성화된 경우 아무 동작도 하지 않음

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
            Managers.SoundMng.PlayItemEffect("Equipment", .4f);
            UnbindEquipment();
            Managers.HeroMng.HeroStorage.UnEquipEquipment(_heroInstanceId, _equipmentType);
            return;
        }
        Managers.SoundMng.PlayItemEffect(equipment.EquipmentData.SoundPath, .4f);
        BindEquipment(equipment);
        Managers.HeroMng.HeroStorage.EquipEquipment(_heroInstanceId, _equipmentType, equipment.InstanceId);
    }

    private void OnDestroy()
    {
        if(_heroInstance != null) _heroInstance.OnEquipmentChanged -= BindEquipment;
    }
}
