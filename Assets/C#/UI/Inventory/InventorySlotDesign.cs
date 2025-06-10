using System.Collections;
using UnityEngine;

/// <summary>
/// 인벤토리 슬롯의 디자인 정보를 보유하는 클래스. 인벤토리 슬롯의 프리팹 경로도 보유.
/// </summary>
public abstract class InventorySlotDesign
{
    /// <summary>
    /// 인벤토리 슬롯 프리팹의 경로를 반환. (UI/SubItemUI/ 이하의 경로)
    /// </summary>
    /// <remarks></remarks>
    public abstract string GetSlotPrefabPath();

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

public class DefaultInventorySlotDesign : InventorySlotDesign
{
    private static Sprite _defaultSlotSprite;
    private static Sprite _slotSpriteOnMouseOver;

    public override string GetSlotPrefabPath() => "Town/UI_InventorySlot_TownInventory";

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

public class HeroEquipmentSlotDesign : InventorySlotDesign
{
    private static Sprite _defaultSlotSprite;
    private static Sprite _slotSpriteOnMouseOver;
    private static Sprite _defaultContentSprite;

    public override string GetSlotPrefabPath() => ""; // 사용 안함

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