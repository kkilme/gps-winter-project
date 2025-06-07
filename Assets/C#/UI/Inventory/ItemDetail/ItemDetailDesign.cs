using System;
using System.Collections;
using System.Text;
using UnityEngine;
using DG.Tweening;


public abstract class ItemDetailDesign
{
    public void Apply(UI_ItemDetailPopup ui, ItemInstanceData itemInstanceData, int quantity = 0)
    {
        // UI_ForceInsideScreen는를 통해 ItemDetailUI의 위치를 매 프레임 설정하는데, 디자인 적용이 끝나야만 ItemDetailUI의 RectTransform 값이 제대로 설정되어 올바른 위치가 결정됨.
        // CanvasGroup을 통해 Design 적용이 끝났을 때 자연스럽게 보이도록 함.
        CanvasGroup canvasGroup = ui.gameObject.GetOrAddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        ApplyDesign(ui, itemInstanceData, quantity);

        canvasGroup.DOFade(1f, 0.2f).SetEase(Ease.OutSine);
    }
    public void Apply(UI_ItemDetailPopup ui, ItemData itemData, int quantity = 0)
    {
        CanvasGroup canvasGroup = ui.gameObject.GetOrAddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        ApplyDesign(ui, itemData, quantity);

        canvasGroup.DOFade(1f, 0.2f).SetEase(Ease.OutSine);
    }

    protected virtual void ApplyDesign(UI_ItemDetailPopup ui, ItemInstanceData itemInstanceData, int quantity = 0)
    {
        ApplyDesign(ui, itemInstanceData.ItemData, quantity);
    }

    protected virtual void ApplyDesign(UI_ItemDetailPopup ui, ItemData itemData, int quantity = 0)
    {
        ApplyName(ui, itemData);
        ApplyItemImage(ui, itemData);
        ApplyDescription(ui, itemData);
        ApplyItemTypeIconImage(ui, itemData);
        ui.SetQuantity(quantity);
        ui.DisableEquippedHeroInfo();
    }

    protected virtual void ApplyName(UI_ItemDetailPopup ui, ItemData itemData)
    {
        ui.SetName(itemData.Name);
    }

    protected virtual void ApplyItemImage(UI_ItemDetailPopup ui, ItemData itemData)
    {
        ui.SetImage(itemData.ImagePath);
    }

    protected abstract void ApplyDescription(UI_ItemDetailPopup ui, ItemData itemData);

    protected abstract void ApplyItemTypeIconImage(UI_ItemDetailPopup ui, ItemData itemData);
}

public abstract class EquipmentItemDetailDesign : ItemDetailDesign
{
    protected override void ApplyDesign(UI_ItemDetailPopup ui, ItemInstanceData itemInstanceData, int quantity = 0)
    {
        base.ApplyDesign(ui, itemInstanceData, quantity);
        ApplyEquippedHeroInfo(ui, itemInstanceData as EquipmentInstanceData);
    }

    protected override void ApplyDescription(UI_ItemDetailPopup ui, ItemData itemData)
    {
        if (itemData is not EquipmentData equipmentData)
        {
            ui.SetDescription("");
            return;
        }

        // 스탯별 증감 정보 서술
        // equipmentData가 같아도 강화 등을 통해 equipmentInstanceData별로 스탯이 다르게 된다면 코드 수정 필요
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

    protected void ApplyEquippedHeroInfo(UI_ItemDetailPopup ui, EquipmentInstanceData equipmentInstanceData)
    {
        if (equipmentInstanceData == null) return;

        if (equipmentInstanceData.EquippedHeroId != -1)
        {
            ui.SetEquippedHeroInfo(equipmentInstanceData.EquippedHeroId);
        }
    }
}

public class WeaponItemDetailDesign : EquipmentItemDetailDesign
{
    protected override void ApplyDescription(UI_ItemDetailPopup ui, ItemData itemData)
    {
        base.ApplyDescription(ui, itemData);
        if(itemData is not WeaponData weaponData) return;

        // 무기 스킬 정보 서술
        StringBuilder sb = new StringBuilder(ui.GetDescription());
        sb.Append("\nSkills: \n");
        foreach (var skill in weaponData.Skills)
        {
            sb.Append($"\t{Managers.DataMng.SkillDataDict[skill].Name}\n");
        }

        ui.SetDescription(sb.ToString().TrimEnd());
    }

    protected override void ApplyItemTypeIconImage(UI_ItemDetailPopup ui, ItemData itemData)
    {
        ui.SetItemTypeIcon("Default_Weapon"); // 무기 타입 별 다른 이미지 지정 가능
    }
}

public class ArmorItemDetailDesign : EquipmentItemDetailDesign
{
    protected override void ApplyItemTypeIconImage(UI_ItemDetailPopup ui, ItemData itemData)
    {
        ui.SetItemTypeIcon("Default_Armor"); // 방어구 부위별 다른 이미지 지정 가능. 맞는 애셋이 없어서 보류.
    }
}

public class ConsumableItemDetailDesign : ItemDetailDesign
{
    protected override void ApplyDescription(UI_ItemDetailPopup ui, ItemData itemData)
    {
        if(itemData is not ConsumableItemData consumableItemData)
        {
            ui.SetDescription("");
            return;
        }

        ui.SetDescription(consumableItemData.Description);
    }

    protected override void ApplyItemTypeIconImage(UI_ItemDetailPopup ui, ItemData itemData)
    {
        ui.SetItemTypeIcon("Default_Consumable");
    }
}

// 골드만을 위한 특수 디자인
public class GoldDetailDesign : ItemDetailDesign
{
    protected override void ApplyDesign(UI_ItemDetailPopup ui, ItemData itemData, int quantity = 0)
    {
        ui.SetName("Gold");
        ui.SetImage("Gold");
        ui.SetDescription("Nobody hates gold, right?");
        ui.SetItemTypeIcon("Gold");
        ui.SetQuantity(quantity);
    }

    // 골드는 itemData가 없으므로 아래 메서드는 사용하지 않음
    protected override void ApplyDescription(UI_ItemDetailPopup ui, ItemData itemData) { }
    protected override void ApplyItemTypeIconImage(UI_ItemDetailPopup ui, ItemData itemData) { }
}