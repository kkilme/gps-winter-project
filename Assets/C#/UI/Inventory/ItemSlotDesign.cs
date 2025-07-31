using UnityEngine;

/* NOTE:
 * 현재 디자인이 필요할 때마다 new()로 생성하는데, 디자인을 캐시하여 재사용할 수 있도록 개선 가능할 것으로 보임.
 * 디자인을 ScriptableObject로 만들 수도 있어보임.
 */
/// <summary>
/// 아이템 슬롯의 디자인 정보를 보유하며, 디자인을 적용한 아이템 슬롯을 생성도 하는 클래스.
/// </summary>
public abstract class ItemSlotDesign
{
    /// <summary>
    /// 커스텀 슬롯인지 여부: 아이템이 들어갈 수 없는 슬롯
    /// </summary>
    protected virtual bool IsCustomSlot => false;

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
    /// static으로 캐시된 스프라이트를 로드하거나, 경로로부터 스프라이트를 로드하여 반환. (유틸 메소드)
    /// </summary>
    protected Sprite LoadSprite(ref Sprite cache, string path)
    {
        if (cache == null)
            cache = Managers.ResourceMng.Load<Sprite>(path);
        return cache;
    }

    /// <summary>
    /// 슬롯의 배경 이미지 스프라이트를 반환.
    /// </summary>
    public abstract Sprite GetDefaultSlotSprite();

    /// <summary>
    /// 마우스 오버 시 변경될 슬롯의 배경 이미지 스프라이트를 반환.
    /// </summary>
    public abstract Sprite GetSlotSpriteOnMouseOver();

    /// <summary>
    /// 기본적으로 사용될 슬롯 내용물 이미지 스프라이트를 반환. 존재하지 않는 경우 null을 반환.
    /// </summary>
    public abstract Sprite GetDefaultContentSprite();
}

public class DefaultItemSlotDesign : ItemSlotDesign
{
    protected static Sprite _defaultSlotSprite;
    protected static Sprite _slotSpriteOnMouseOver;
    protected static Sprite _defaultContentSprite;

    public override Sprite GetDefaultSlotSprite()
    {
        return LoadSprite(ref _defaultSlotSprite, "Textures/Inventory/ItemSlot_Default");
    }

    public override Sprite GetSlotSpriteOnMouseOver()
    {
        return LoadSprite(ref _slotSpriteOnMouseOver, "Textures/Inventory/ItemSlot_Mouseover_Default");
    }

    public override Sprite GetDefaultContentSprite()
    {
        return null;
    }
}

public class PlusIconItemSlotDesign : ItemSlotDesign
{
    protected static Sprite _defaultSlotSprite;
    protected static Sprite _slotSpriteOnMouseOver;
    protected static Sprite _defaultContentSprite;


    public override Sprite GetDefaultSlotSprite()
    {
        return LoadSprite(ref _defaultSlotSprite, "Textures/Inventory/ItemSlot_Default");
    }

    public override Sprite GetSlotSpriteOnMouseOver()
    {
        return LoadSprite(ref _slotSpriteOnMouseOver, "Textures/Inventory/ItemSlot_Mouseover_Default");
    }

    public override Sprite GetDefaultContentSprite()
    {
        return LoadSprite(ref _defaultContentSprite, "Textures/PictoIcons/PictoIcon_Plus");
    }
}

public class EquipmentUnequipSlotDesign : ItemSlotDesign
{
    protected override bool IsCustomSlot => true;
    protected static Sprite _defaultSlotSprite;
    protected static Sprite _slotSpriteOnMouseOver;
    protected static Sprite _defaultContentSprite;

    public override Sprite GetDefaultSlotSprite()
    {
        return LoadSprite(ref _defaultSlotSprite, "Textures/Inventory/ItemSlot_Default");
    }

    public override Sprite GetSlotSpriteOnMouseOver()
    {
        return LoadSprite(ref _slotSpriteOnMouseOver, "Textures/Inventory/ItemSlot_Mouseover_Default");
    }

    public override Sprite GetDefaultContentSprite()
    {
        return LoadSprite(ref _defaultContentSprite, "Textures/PictoIcons/PictoIcon_X");
    }
}

public class QuestRewardSlotDesign : ItemSlotDesign
{
    protected override bool IsCustomSlot => true;
    protected static Sprite _defaultSlotSprite;
    protected static Sprite _slotSpriteOnMouseOver;
    protected static Sprite _defaultContentSprite;

    public override Sprite GetDefaultSlotSprite()
    {
        return LoadSprite(ref _defaultSlotSprite, "Textures/Inventory/ItemSlot_GreenFrame");
    }

    public override Sprite GetSlotSpriteOnMouseOver()
    {
        return null;
    }

    public override Sprite GetDefaultContentSprite()
    {
        return null;
    }
}