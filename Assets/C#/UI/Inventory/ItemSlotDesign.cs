using System.Collections;
using UnityEngine;

/// <summary>
/// 아이템 슬롯의 디자인 정보를 보유하며, 아이템 슬롯을 생성하는 클래스.
/// </summary>
public abstract class ItemSlotDesign
{
    /// <summary>
    /// 커스텀 슬롯인지 여부: 아이템이 들어갈 수 없는 슬롯
    /// </summary>
    protected abstract bool IsCustomSlot { get; set; }

    /// <summary>
    /// 아이템 슬롯을 생성하여, 초기화 및 디자인을 적용 후 리턴.
    /// </summary>
    public virtual UI_ItemSlot CreateItemSlot(Transform parent = null)
    {
        UI_ItemSlot slot = Managers.UIMng.MakeGeneralUI<UI_ItemSlot>(parent);
        slot.LateInit(this, IsCustomSlot);
        return slot;
    }

    /// <summary>
    /// 슬롯의 배경 이미지 스프라이트를 반환.
    /// </summary>
    public abstract Sprite GetDefaultSlotSprite();

    /// <summary>
    /// 마우스 오버 시 변경될 슬롯의 배경 이미지 스프라이트를 반환.
    /// </summary>
    /// <returns></returns>
    public abstract Sprite GetSlotSpriteOnMouseOver();

    /// <summary>
    /// 기본적으로 사용될 슬롯 내용물 이미지 스프라이트를 반환. 존재하지 않는 경우 null을 반환.
    /// </summary>
    public abstract Sprite GetDefaultContentSprite();

}

public class DefaultItemSlotDesign : ItemSlotDesign
{
    protected override bool IsCustomSlot { get; set; } = false;
    protected static Sprite _defaultSlotSprite;
    protected static Sprite _slotSpriteOnMouseOver;
    protected static Sprite _defaultContentSprite;

    public override Sprite GetDefaultSlotSprite()
    {
        if (_defaultSlotSprite == null)
            _defaultSlotSprite = Managers.ResourceMng.Load<Sprite>("Textures/Inventory/ItemSlot_Default");
        return _defaultSlotSprite;
    }

    public override Sprite GetSlotSpriteOnMouseOver()
    {
        if (_slotSpriteOnMouseOver == null)
            _slotSpriteOnMouseOver = Managers.ResourceMng.Load<Sprite>("Textures/Inventory/ItemSlot_Mouseover_Default");
        return _slotSpriteOnMouseOver;
    }

    public override Sprite GetDefaultContentSprite()
    {
        return null;
    }
}

public class HeroEquipmentSlotDesign : ItemSlotDesign
{
    protected override bool IsCustomSlot { get; set; } = false;
    protected static Sprite _defaultSlotSprite;
    protected static Sprite _slotSpriteOnMouseOver;
    protected static Sprite _defaultContentSprite;


    public override Sprite GetDefaultSlotSprite()
    {
        if (_defaultSlotSprite == null)
            _defaultSlotSprite = Managers.ResourceMng.Load<Sprite>("Textures/Inventory/ItemSlot_Default");
        return _defaultSlotSprite;
    }

    public override Sprite GetSlotSpriteOnMouseOver()
    {
        if (_slotSpriteOnMouseOver == null)
            _slotSpriteOnMouseOver = Managers.ResourceMng.Load<Sprite>("Textures/Inventory/ItemSlot_Mouseover_Default");
        return _slotSpriteOnMouseOver;
    }

    public override Sprite GetDefaultContentSprite()
    {
        if(_defaultContentSprite == null)
            _defaultContentSprite = Managers.ResourceMng.Load<Sprite>("Textures/PictoIcons/PictoIcon_Plus");
        return _defaultContentSprite;
    }
}

public class EquipmentUnequipSlotDesign : ItemSlotDesign
{
    protected override bool IsCustomSlot { get; set; } = true;
    protected static Sprite _defaultSlotSprite;
    protected static Sprite _slotSpriteOnMouseOver;
    protected static Sprite _defaultContentSprite;

    public override Sprite GetDefaultSlotSprite()
    {
        if (_defaultSlotSprite == null)
            _defaultSlotSprite = Managers.ResourceMng.Load<Sprite>("Textures/Inventory/ItemSlot_Default");
        return _defaultSlotSprite;
    }

    public override Sprite GetSlotSpriteOnMouseOver()
    {
        if (_slotSpriteOnMouseOver == null)
            _slotSpriteOnMouseOver = Managers.ResourceMng.Load<Sprite>("Textures/Inventory/ItemSlot_Mouseover_Default");
        return _slotSpriteOnMouseOver;
    }

    public override Sprite GetDefaultContentSprite()
    {
        if (_defaultContentSprite == null)
            _defaultContentSprite = Managers.ResourceMng.Load<Sprite>("Textures/PictoIcons/PictoIcon_X");
        return _defaultContentSprite;
    }
}