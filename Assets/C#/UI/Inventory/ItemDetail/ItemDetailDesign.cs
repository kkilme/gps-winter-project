using System;
using System.Collections;
using System.Text;
using UnityEngine;


public abstract class ItemDetailDesign
{
    public virtual void Apply(UI_ItemDetail ui, ItemData itemData, int quantity = 0)
    {
        ApplyName(ui, itemData);
        ApplyItemImage(ui, itemData);
        ApplyDescription(ui, itemData);
        ApplyItemTypeIconImage(ui, itemData);
        ui.SetQuantity(quantity);
    }

    protected virtual void ApplyName(UI_ItemDetail ui, ItemData itemData)
    {
        ui.SetName(itemData.Name);
    }

    protected virtual void ApplyItemImage(UI_ItemDetail ui, ItemData itemData)
    {
        ui.SetImage(itemData.ImagePath);
    }

    protected abstract void ApplyDescription(UI_ItemDetail ui, ItemData itemData);

    protected abstract void ApplyItemTypeIconImage(UI_ItemDetail ui, ItemData itemData);   
}

public abstract class EquipmentItemDetailDesign : ItemDetailDesign
{
    protected override void ApplyDescription(UI_ItemDetail ui, ItemData itemData)
    {
        if (itemData is not EquipmentData equipmentData)
        {
            ui.SetDescription("");
            return;
        }

        (string statName, Func<EquipmentData, int> getter)[] stats = new (string, Func<EquipmentData, int>)[]
        {
            ("MaxHp", e => e.Hp),
            ("BaseDamage", e => e.Attack),
            ("PhysicalDefense", e => e.PhysicalDefense),
            ("MagicDefense", e => e.MagicDefense),
            ("Dexterity", e => e.Dexterity),
            ("Strength", e => e.Strength),
            ("Vitality", e => e.Vitality),
            ("Intelligence", e => e.Intelligence),
        };

        StringBuilder sb = new StringBuilder();

        foreach (var (statName, getter) in stats)
        {
            int value = getter(equipmentData);
            if (value == 0) continue;

            string color = value > 0 ? "#3A8DFF" : "#FF3A3A"; // 파랑/빨강
            string sign = value > 0 ? "+" : "-";
            int absValue = Math.Abs(value);

            sb.Append($"<color={color}>{statName} {sign}{absValue}</color>\n");
        }

        ui.SetDescription(sb.ToString().TrimEnd());
    }
}

public class WeaponItemDetailDesign : EquipmentItemDetailDesign
{
    protected override void ApplyDescription(UI_ItemDetail ui, ItemData itemData)
    {
        base.ApplyDescription(ui, itemData);
        if(itemData is not WeaponData weaponData) return;

        StringBuilder sb = new StringBuilder(ui.GetDescription());
        sb.Append("\nSkills: \n");
        foreach (var skill in weaponData.Skills)
        {
            sb.Append($"\t{Managers.DataMng.SkillDataDict[skill].Name}\n");
        }

        ui.SetDescription(sb.ToString().TrimEnd());
    }

    protected override void ApplyItemTypeIconImage(UI_ItemDetail ui, ItemData itemData)
    {
        ui.SetItemTypeIcon("Default_Weapon"); // 무기 타입 별 다른 이미지 지정 가능
    }
}

public class ArmorItemDetailDesign : EquipmentItemDetailDesign
{
    protected override void ApplyItemTypeIconImage(UI_ItemDetail ui, ItemData itemData)
    {
        ui.SetItemTypeIcon("Default_Armor"); // 방어구 부위별 다른 이미지 지정 가능. 맞는 애셋이 없어서 보류.
    }
}

public class ConsumableItemDetailDesign : ItemDetailDesign
{
    protected override void ApplyDescription(UI_ItemDetail ui, ItemData itemData)
    {
        if(itemData is not ConsumableItemData consumableItemData)
        {
            ui.SetDescription("");
            return;
        }

        ui.SetDescription(consumableItemData.Description);
    }

    protected override void ApplyItemTypeIconImage(UI_ItemDetail ui, ItemData itemData)
    {
        ui.SetItemTypeIcon("Default_Consumable");
    }
}

// 골드만을 위한 특수 디자인
public class GoldDetailDesign : ItemDetailDesign
{
    public override void Apply(UI_ItemDetail ui, ItemData itemData, int quantity = 0)
    {
        ui.SetName("Gold");
        ui.SetImage("Gold");
        ui.SetDescription("Nobody hates gold, right?");
        ui.SetItemTypeIcon("Gold");
        ui.SetQuantity(quantity);
    }

    // 골드는 itemData가 없으므로 아래 메서드는 사용하지 않음
    protected override void ApplyDescription(UI_ItemDetail ui, ItemData itemData) { }
    protected override void ApplyItemTypeIconImage(UI_ItemDetail ui, ItemData itemData) { }
}