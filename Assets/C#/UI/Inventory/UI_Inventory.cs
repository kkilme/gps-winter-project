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
    public List<UI_InventorySlot> InventorySlots { get; private set; } = new(); // 인벤토리 크기 제한은 현재 없음

    private Action<UI_InventorySlot> _onSlotClickAction; // 슬롯 클릭 시 호출되는 액션. 새 슬롯 추가될 때 사용할 수 있도록 저장해둠.
    private InventorySlotDesign _slotDesign; // 슬롯의 디자인 정보. 기본적으로는 DefaultInventorySlotDesign 사용.

    private ScrollRect _scrollRect; // 이 인벤토리를 컨텐츠로 가지는 ScrollRect

    public override void Init() { }

    public void LateInit(Action<UI_InventorySlot> onSlotClickAction = null, InventorySlotDesign slotDesign = null) 
    {
        _scrollRect = GetComponentInParent<ScrollRect>();

        ShowInstantly(); // 일부 요소는 게임 오브젝트가 비활성화 상태일 시 초기화가 실패하므로 꼭 활성화해주어야 함
        _onSlotClickAction = onSlotClickAction;

        slotDesign ??= new DefaultInventorySlotDesign(); // 디자인을 지정하지 않을 시 기본 슬롯 디자인 사용
        _slotDesign = slotDesign;


        // 이미 존재하는 슬롯들 할당 및 초기화
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            UI_InventorySlot slot = gameObject.transform.GetChild(i).GetOrAddComponent<UI_InventorySlot>();
            slot.LateInit(onSlotClickAction, slotDesign);
            InventorySlots.Add(slot);
        }

        HideInstantly();
    }

    /// <summary>
    /// 새로운 slot을 생성하여 자식으로 추가.
    /// </summary>
    public void AddEmptySlot()
    {
        UI_InventorySlot slot = Managers.UIMng.MakeSubItemUI<UI_InventorySlot>(transform, _slotDesign.GetSlotPrefabPath());
        slot.LateInit(_onSlotClickAction, _slotDesign);
        InventorySlots.Add(slot);
    }

    /// <summary>
    /// 커스텀 슬롯 추가. 주로 아이템이 아닌 요소를 슬롯에 할당하기 위해 사용.
    /// </summary>
    /// <param name="onClickAction">커스텀 슬롯 클릭 시 실행 될 Action</param>
    /// <param name="contentSprite">커스텀 슬롯(내용물) 이미지의 Sprite</param>
    public void AddCustomSlot(Action<UI_InventorySlot> onClickAction, Sprite contentSprite = null)
    {
        UI_InventorySlot emptySlot = InventorySlots.Find(s => s.IsEmpty);
        if (emptySlot == null)
        {
            AddEmptySlot();
            emptySlot = InventorySlots[^1];
        }

        emptySlot.LateInit(onClickAction, _slotDesign, true);
        emptySlot.ForceSetContentSprite(contentSprite);
    }

    /// <summary>
    /// 적절한 슬롯을 찾거나 생성하여 인벤토리 슬롯에 아이템 인스턴스를 바인딩.
    /// </summary>
    public void AddItem(ItemInstanceData itemInstanceData)
    {
        int left = itemInstanceData.Quantity; // 장비의 경우 1로 고정

        // 먼저 스택 가능한 슬롯을 찾고 추가
        foreach (var slot in InventorySlots)
        {
            if (!slot.IsEmpty && slot.ItemInstanceData == itemInstanceData && slot.Quantity < slot.ItemData.MaxStack)
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

            int toAdd = Mathf.Min(itemInstanceData.ItemData.MaxStack, left);
            emptySlot.BindItem(itemInstanceData, toAdd);
            left -= toAdd;
        }

        ValidateInventory(); // 아이템 추가 후 인벤토리 검증
    }

    /// <summary>
    /// 인벤토리 슬롯 UI의 각 아이템 인스턴스별로, 실제 인벤토리 매니저의 데이터와 수량이 일치하는지 검증하고, 초과 시 UI의 수량을 조정.
    /// </summary>
    public void ValidateInventory()
    {
        // 인벤토리 슬롯에 바인딩된 모든 아이템을 인스턴스별로 그룹화
        Dictionary<int, List<UI_InventorySlot>> slotsByInstanceId = new(); // key: ItemInstanceData의 InstanceId, value: UI_InventorySlot 리스트
        foreach (var slot in InventorySlots)
        {
            if (slot.IsEmpty || slot.ItemInstanceData == null)
                continue;

            int instanceId = slot.ItemInstanceData.InstanceId;
            if (!slotsByInstanceId.ContainsKey(instanceId))
                slotsByInstanceId[instanceId] = new List<UI_InventorySlot>();
            slotsByInstanceId[instanceId].Add(slot);
        }

        // 각 인스턴스별로 실제 인벤토리 매니저의 수량과 비교
        foreach (var kvp in slotsByInstanceId)
        {
            int instanceId = kvp.Key;
            List<UI_InventorySlot> slots = kvp.Value;

            // 실제 인벤토리 매니저의 데이터에서 해당 인스턴스의 수량을 가져옴
            if (!Managers.InvMng.ItemDict.TryGetValue(instanceId, out ItemInstanceData realData))
            {
                // 매니저에 없는 인스턴스는 UI에서 언바인드
                foreach (var slot in slots)
                    slot.UnbindItem();
                continue;
            }

            int realQuantity = realData.Quantity;
            int uiQuantitySum = 0;
            foreach (var slot in slots)
                uiQuantitySum += slot.Quantity;

            // UI에 바인딩된 총 수량이 실제보다 많으면, 초과분만큼 UI에서 차감
            if (uiQuantitySum > realQuantity)
            {
                int over = uiQuantitySum - realQuantity;
                // 뒤에서부터(마지막 슬롯부터) 차감
                for (int i = slots.Count - 1; i >= 0 && over > 0; i--)
                {
                    var slot = slots[i];
                    int reduce = Math.Min(slot.Quantity, over);
                    slot.Quantity -= reduce;
                    over -= reduce;
                }
            }
        }
    }

    /// <summary>
    /// 모든 인벤토리 슬롯에서 바인딩된 클릭 액션 모두 제거.
    /// </summary>
    public void ClearActionCallbackOnSlots()
    {
        foreach (var slot in InventorySlots)
        {
            slot.OnClickAction = null;
        }
    }

    /// <summary>
    /// 모든 인벤토리 슬롯에서 바인딩 된 특정 클릭 액션 제거.
    /// </summary>
    public void RemoveActionCallbackOnSlots(Action<UI_InventorySlot> action)
    {
        foreach (var slot in InventorySlots)
        {
            slot.OnClickAction -= action;
        }
    }

    public override void ShowInstantly()
    {
        base.ShowInstantly();
        _scrollRect.content = gameObject.transform as RectTransform;

        // 스크롤 위치를 맨 위로 초기화
        _scrollRect.verticalNormalizedPosition = 1f;
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
