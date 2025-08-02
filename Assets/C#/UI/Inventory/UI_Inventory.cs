using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임에서 사용되는 모든 인벤토리 형식 UI에 사용
/// </summary>
[RequireComponent(typeof(GridLayoutGroup))]
public class UI_Inventory : UI_Base
{
    public List<UI_ItemSlot> InventorySlots { get; private set; } = new();
    private int _maxSize; // 인벤토리의 최대 슬롯 개수. 기본값은 9999로 설정되어 있으나, 필요에 따라 변경 가능.

    private Action<UI_ItemSlot> _onSlotClickAction; // 슬롯 클릭 시 호출되는 액션. 새 슬롯 추가될 때 재사용할 수 있도록 저장해둠.
    private ItemSlotDesign _slotDesign; // 슬롯의 디자인 정보. 기본적으로는 DefaultItemSlotDesign 사용.

    private ScrollRect _scrollRect; // 이 인벤토리를 컨텐츠로 가지는 ScrollRect (null일 경우, 스크롤이 없는 인벤토리 UI)

    public override void Init() { }

    public void LateInit(Action<UI_ItemSlot> onSlotClickAction = null, ItemSlotDesign slotDesign = null, int maxSize = 9999)
    {
        _scrollRect = GetComponentInParent<ScrollRect>();
        _maxSize = maxSize;

        ShowInstantly(); // 일부 요소는 게임 오브젝트가 비활성화 상태일 시 초기화가 실패하므로 꼭 활성화해주어야 함. 이후 다시 비활성화는 LateInit을 호출하는 클래스의 몫임.
        _onSlotClickAction = onSlotClickAction;

        slotDesign ??= new DefaultItemSlotDesign(); // 디자인을 지정하지 않을 시 기본 슬롯 디자인 사용
        _slotDesign = slotDesign;

        // 이미 존재하는 슬롯들 할당 및 초기화
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            UI_ItemSlot slot = gameObject.transform.GetChild(i).GetOrAddComponent<UI_ItemSlot>();
            slot.LateInit(slotDesign);
            slot.OnClickAction -= _onSlotClickAction;
            slot.OnClickAction += _onSlotClickAction;
            InventorySlots.Add(slot);
        }
    }

    /// <summary>
    /// 새로운 slot을 생성하여 자식으로 추가.
    /// </summary>
    public void AddEmptySlot()
    {
        UI_ItemSlot slot = _slotDesign.CreateItemSlot(transform);
        slot.OnClickAction -= _onSlotClickAction;
        slot.OnClickAction += _onSlotClickAction;
        InventorySlots.Add(slot);
    }

    /// <summary>
    /// 외부에서 생성한 슬롯 추가. 주로 아이템이 아닌 요소를 커스텀으로 할당한 슬롯을 추가하기 위해 사용. 인벤토리 최대 크기를 무시함.
    /// </summary>
    public void AddSlot(UI_ItemSlot itemSlot, bool addDefaultActionCallback = false)
    {
        if (addDefaultActionCallback)
        {
            itemSlot.OnClickAction -= _onSlotClickAction;
            itemSlot.OnClickAction += _onSlotClickAction;
        }
        InventorySlots.Add(itemSlot);
    }

    /// <summary>
    /// 적절한 슬롯을 찾거나 생성하여 인벤토리 슬롯에 아이템 인스턴스 데이터를 바인딩.
    /// </summary>
    public void AddItem(ItemInstanceData itemInstanceData)
    {
        if (itemInstanceData == null || itemInstanceData.Quantity <= 0) return;

        int left = itemInstanceData.Quantity;
        int maxStack = itemInstanceData.ItemData.MaxStack;

        // 빈 슬롯을 찾거나 만들어서 추가
        while (left > 0)
        {
            if (IsFull())
            {
                Debug.LogWarning($"[UI_Inventory] Could not add {itemInstanceData.ItemData.Name} * {left}. Inventory is full!");
                break;
            }

            UI_ItemSlot emptySlot = InventorySlots.Find(s => s.IsEmpty);
            if (emptySlot == null)
            {
                AddEmptySlot();
                emptySlot = InventorySlots[^1];
            }

            int toAdd = Mathf.Min(maxStack, left);
            emptySlot.BindItem(itemInstanceData, toAdd);
            left -= toAdd;
        }

        ValidateInventory(); // 아이템 추가 후 인벤토리 검증
    }

    /// <summary>
    /// 적절한 슬롯을 찾거나 생성하여 인벤토리 슬롯에 아이템 데이터를 바인딩.
    /// </summary>
    /// <param name="enableStack"> 다른 칸에 같은 아이템이 있을 경우 스택을 할지 여부. 기본값은 false.</param>
    public void AddItem(ItemData itemData, int quantity = 1, bool enableStack = false)
    {
        if (itemData == null || quantity <= 0) return;

        if (enableStack) // 스택을 허용하는 경우
        {
            int left = quantity;
            // 스택 가능한 슬롯을 찾고 추가
            foreach (var slot in InventorySlots)
            {
                if (!slot.IsEmpty && slot.ItemData == itemData && slot.Quantity < itemData.MaxStack)
                {
                    int canAdd = itemData.MaxStack - slot.Quantity;
                    int toAdd = Mathf.Min(canAdd, left);
                    slot.Quantity += toAdd;
                    left -= toAdd;
                    if (left <= 0)
                        return;
                }
            }

            // 빈 슬롯을 찾거나 만들어서 추가
            while (left > 0)
            {
                if (IsFull())
                {
                    Debug.LogWarning($"[UI_Inventory] Could not add {itemData.Name} * {left}. Inventory is full!");
                    break;
                }

                UI_ItemSlot emptySlot = InventorySlots.Find(s => s.IsEmpty);
                if (emptySlot == null)
                {
                    AddEmptySlot();
                    emptySlot = InventorySlots[^1];
                }

                int toAdd = Mathf.Min(itemData.MaxStack, left);
                emptySlot.BindItem(itemData, toAdd);
                left -= toAdd;
            }
        }
        else // 스택을 하지 않고 새로운 슬롯에 quantity만큼 추가
        {
            UI_ItemSlot emptySlot = InventorySlots.Find(s => s.IsEmpty);
            if (emptySlot == null)
            {
                AddEmptySlot();
                emptySlot = InventorySlots[^1];
            }
            emptySlot.BindItem(itemData, quantity);
        }

    }

    /// <summary>
    /// itemInstanceData를 인벤토리에서 제거
    /// </summary>
    public void RemoveItem(ItemInstanceData itemInstanceData, bool destroySlot = false)
    {
        if (itemInstanceData == null) return;

        // 스택 가능한 슬롯을 찾고 추가
        foreach (var slot in InventorySlots)
        {
            if (!slot.IsEmpty && slot.ItemInstanceData == itemInstanceData)
            {
                slot.UnbindItem();
                if (destroySlot)
                {
                    InventorySlots.Remove(slot);
                    Destroy(slot.gameObject); // 슬롯 오브젝트 파괴
                }
            }
        }
    }

    /// <summary>
    /// itemData를 quantity만큼 인벤토리에서 제거 (뒤 슬롯부터 제거)
    /// </summary>
    public void RemoveItem(ItemData itemData, int quantity = 1, bool destroySlot = false)
    {
        if (itemData == null) return;

        while (quantity > 0)
        {
            // 뒤에서부터 찾기
            UI_ItemSlot slot = null;
            for (int i = InventorySlots.Count - 1; i >= 0; i--)
            {
                if (InventorySlots[i].ItemData == itemData)
                {
                    slot = InventorySlots[i];
                    break;
                }
            }
            if (slot == null)
            {
                Debug.LogWarning($"[UI_Inventory] Could not remove item: {itemData.Name}) * {quantity}. Does not exist in inventory!");
                break;
            }
            if (quantity >= slot.Quantity)
            {
                quantity -= slot.Quantity;
                slot.UnbindItem();
                if (destroySlot)
                {
                    InventorySlots.Remove(slot);
                    Destroy(slot.gameObject); // 슬롯 오브젝트 파괴
                }
            }
            else
            {
                slot.Quantity -= quantity;
                quantity = 0;
            }
        }
    }

    /// <summary>
    /// 특정 itemSlot에 바인딩된 item을 quantity만큼 제거.
    /// </summary>
    /// <remarks> 해당 슬롯의 Quantity가 제거하고자 하는 quantity보다 적다면 무시됨.</remarks>
    public void RemoveItem(UI_ItemSlot itemSlot, int quantity = 1, bool destroySlot = false)
    {
        if (itemSlot == null) return;

        UI_ItemSlot slot = InventorySlots.Find(s => s == itemSlot);
        if (slot == null || slot.IsEmpty || slot.Quantity < quantity) return;

        slot.Quantity -= quantity;
        if (slot.Quantity <= 0)
        {
            slot.UnbindItem();
            if (destroySlot)
            {
                InventorySlots.Remove(slot);
                Destroy(slot.gameObject); // 슬롯 오브젝트 파괴
            }
        }
    }

    /// <summary>
    /// 특정 슬롯에서 아이템 바인딩 해제.
    /// </summary>
    public void UnbindSlot(UI_ItemSlot itemSlot, bool destroySlot = false)
    {
        int idx = InventorySlots.FindIndex(slot => slot == itemSlot);
        if (idx >= 0)
        {
            itemSlot.UnbindItem();
            if (destroySlot)
            {
                InventorySlots.Remove(itemSlot);
                Destroy(itemSlot.gameObject); // 슬롯 오브젝트 파괴
            }
        }
    }

    /// <summary>
    /// 인벤토리 슬롯 UI의 각 아이템 인스턴스별로, 실제 인벤토리 매니저의 데이터 및 수량과 일치하는지 검증하고, 초과 시 UI 강제 조정.
    /// </summary>
    public void ValidateInventory()
    {
        // 인벤토리 슬롯에 바인딩된 모든 아이템을 인스턴스별로 그룹화
        Dictionary<int, List<UI_ItemSlot>> slotsByInstanceId = new(); // key: ItemInstanceData의 InstanceId, value: UI_ItemSlot 리스트
        foreach (var slot in InventorySlots)
        {
            if (slot.IsEmpty || slot.ItemInstanceData == null)
                continue;

            int instanceId = slot.ItemInstanceData.InstanceId;
            if (!slotsByInstanceId.ContainsKey(instanceId))
                slotsByInstanceId[instanceId] = new List<UI_ItemSlot>();
            slotsByInstanceId[instanceId].Add(slot);
        }

        // 각 인스턴스별로 실제 인벤토리 매니저의 수량과 비교
        foreach (var kvp in slotsByInstanceId)
        {
            int instanceId = kvp.Key;
            List<UI_ItemSlot> slots = kvp.Value;

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
    /// 인벤토리가 가득 차있는지 여부. 즉, 아이템이 바인딩 된 슬롯 개수가 인벤토리의 _maxSize 이상인지 여부.
    /// </summary>
    public bool IsFull()
    {
        int notEmptySlotCnt = InventorySlots.Count(slot => !slot.IsEmpty);
        return notEmptySlotCnt >= _maxSize;
    }

    /// <summary>
    /// 인벤토리가 비어있는지 여부. 즉, 아이템이 바인딩 된 슬롯이 하나라도 존재하는 지 여부.
    /// </summary>
    public bool IsEmpty()
    {
        int notEmptySlotCnt = InventorySlots.Count(slot => !slot.IsEmpty);
        return notEmptySlotCnt == 0;
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
    public void RemoveActionCallbackOnSlots(Action<UI_ItemSlot> action)
    {
        foreach (var slot in InventorySlots)
        {
            slot.OnClickAction -= action;
        }
    }

    public override void ShowInstantly()
    {
        base.ShowInstantly();
        if (_scrollRect) _scrollRect.content = gameObject.transform as RectTransform;

        // 스크롤 위치를 맨 위로 초기화
        if (_scrollRect) _scrollRect.verticalNormalizedPosition = 1f;
    }

    /// <summary>
    /// 인벤토리의 모든 아이템 슬롯을 Unbind.
    /// </summary>
    /// <param name="removeActionCallback">
    /// 슬롯의 Callback도 함께 지울 지 여부
    /// </param>
    public void Clear(bool removeActionCallback = true)
    {
        if (removeActionCallback) ClearActionCallbackOnSlots();
        foreach (var slot in InventorySlots)
        {
            slot.UnbindItem();
        }
    }

    /// <summary>
    /// 인벤토리의 모든 아이템 슬롯 오브젝트를 파괴.
    /// </summary>
    public void HardClear()
    {
        ClearActionCallbackOnSlots();
        while (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            child.SetParent(null);      // 부모 자식 관계 해제 -> childCount 즉시 감소
            Destroy(child.gameObject);  // 실제 파괴는 다음 프레임에
        }
        InventorySlots.Clear();
    }

    public void OnDestroy()
    {
        ClearActionCallbackOnSlots();
    }
}
