using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임에서 사용되는 모든 인벤토리 형식 UI에 사용
/// </summary>
[RequireComponent(typeof(GridLayoutGroup))]
public class UI_Inventory : UI_Base
{
    private List<UI_InventorySlot> _inventorySlots = new(); // slot의 제한(인벤토리 크기)은 현재 없음

    private string _inventorySlotPath = "Town/UI_InventorySlot_TownInventory"; // UI/SubItemUI/ 이하 경로

    public override void Init() { }

    public void LateInit()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            UI_InventorySlot slot = gameObject.transform.GetChild(i).GetOrAddComponent<UI_InventorySlot>();
            slot.LateInit();
            _inventorySlots.Add(slot);
        }
    }
    /// <summary>
    /// 인벤토리별 slot의 디자인(프리팹)이 다를 수 있음. 그럴 시 외부에서 새롭게 set 필요.
    /// </summary>
    public void SetSlotPath(string path)
    {
        _inventorySlotPath = path;
    }

    /// <summary>
    /// 새로운 slot을 생성하여 자식으로 추가.
    /// </summary>
    public void AddEmptySlot()
    {
        UI_InventorySlot slot = Managers.UIMng.MakeSubItemUI<UI_InventorySlot>(transform, _inventorySlotPath);
        slot.LateInit();
        _inventorySlots.Add(slot);
    }

    /// <summary>
    /// 적절한 슬롯을 찾거나 생성하여 인벤토리 슬롯에 아이템을 바인딩.
    /// </summary>
    public void AddItem(ItemData item, int quantity = 1)
    {
        int left = quantity;

        // 먼저 스택 가능한 슬롯을 찾고 추가
        foreach (var slot in _inventorySlots)
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
            UI_InventorySlot emptySlot = _inventorySlots.Find(s => s.IsEmpty);
            if (emptySlot == null)
            {
                AddEmptySlot();
                emptySlot = _inventorySlots[^1];
            }

            int toAdd = Mathf.Min(item.MaxStack, left);
            emptySlot.BindItem(item, toAdd);
            left -= toAdd;
        }
    }

    public override void HideInstantly()
    {
        Clear();
        base.HideInstantly();
    }

    public void Clear()
    {
        foreach (var slot in _inventorySlots)
        {
            slot.UnbindItem();
        }
        Debug.Log(gameObject.name + " Clear");
    }

}
