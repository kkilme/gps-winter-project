using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 게임에서 사용되는 모든 인벤토리 형식 UI에 사용
/// </summary>
[RequireComponent(typeof(GridLayoutGroup))]
public class UI_Inventory : UI_Base
{
    public List<UI_InventorySlot> InventorySlots { get; private set; } = new(); // slot의 제한(인벤토리 크기)은 현재 없음

    private Action<UI_InventorySlot> _onSlotClickAction; // 슬롯 클릭 시 호출되는 액션

    private string _inventorySlotPath; // UI/SubItemUI/ 이하의 인벤토리 슬롯 프리팹 경로
    private string _slotSpriteOnMouseEnterPath; // 슬롯 마우스 오버 시 이미지 경로

    public override void Init() { }

    public void LateInit(Action<UI_InventorySlot> onSlotClickAction = null,
                        string slotPrefabPath = "Town/UI_InventorySlot_TownInventory", 
                        string slotSpriteOnMouseEnterPath = "Textures/Others/ItemSlot_Selected")
    {
        _onSlotClickAction = onSlotClickAction;
        _inventorySlotPath = slotPrefabPath;
        _slotSpriteOnMouseEnterPath = slotSpriteOnMouseEnterPath;

        // 이미 존재하는 슬롯들 할당 및 초기화
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            UI_InventorySlot slot = gameObject.transform.GetChild(i).GetOrAddComponent<UI_InventorySlot>();
            slot.LateInit(slotSpriteOnMouseEnterPath, onSlotClickAction);
            InventorySlots.Add(slot);
        }
    }

    /// <summary>
    /// 새로운 slot을 생성하여 자식으로 추가.
    /// </summary>
    public void AddEmptySlot()
    {
        UI_InventorySlot slot = Managers.UIMng.MakeSubItemUI<UI_InventorySlot>(transform, _inventorySlotPath);
        slot.LateInit(_slotSpriteOnMouseEnterPath, _onSlotClickAction);
        InventorySlots.Add(slot);
    }

    /// <summary>
    /// 적절한 슬롯을 찾거나 생성하여 인벤토리 슬롯에 아이템을 바인딩.
    /// </summary>
    public void AddItem(ItemData item, int quantity = 1)
    {
        int left = quantity;

        // 먼저 스택 가능한 슬롯을 찾고 추가
        foreach (var slot in InventorySlots)
        {
            if (!slot.IsEmpty && slot.ItemData == item && slot.Quantity < slot.ItemData.MaxStack)
            {
                int canAdd = slot.ItemData.MaxStack - slot.Quantity;
                int toAdd = Mathf.Min(canAdd, left);
                slot.Quantity += toAdd;
                left -= toAdd;
                if (left <= 0)
                    return;
            }
        }

        // 남은 수량이 있으면 빈 슬롯을 찾거나 만들어서 추가
        while (left > 0)
        {
            UI_InventorySlot emptySlot = InventorySlots.Find(s => s.IsEmpty);
            if (emptySlot == null)
            {
                AddEmptySlot();
                emptySlot = InventorySlots[^1];
            }

            int toAdd = Mathf.Min(item.MaxStack, left);
            emptySlot.BindItem(item, toAdd);
            left -= toAdd;
        }
    }

    public void RemoveActionCallbackOnSlots(Action<UI_InventorySlot> action)
    {
        foreach (var slot in InventorySlots)
        {
            slot.OnClickAction -= action;
        }
    }

    public override void HideInstantly()
    {
        Clear();
        base.HideInstantly();
    }

    public void Clear()
    {
        foreach (var slot in InventorySlots)
        {
            slot.UnbindItem();
        }
    }

}
