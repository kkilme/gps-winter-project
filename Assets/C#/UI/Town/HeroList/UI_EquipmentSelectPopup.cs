using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 영웅에게 장착할 장비 선택에 사용되는 인벤토리형 팝업 UI.
/// </summary>
public class UI_EquipmentSelectPopup : UI_Popup
{
    enum GameObjects
    {
        Inventory
    }

    enum Buttons
    {
        Button_Close
    }

    private UI_Inventory _inventory; // 장착할 수 있는 장비가 담긴 인벤토리 (이 팝업 UI의 인벤토리)
    private UI_HeroEquipmentSlot _bindingEquipmentSlot; // 장착할 장비 슬롯

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        GetButton(Buttons.Button_Close).onClick.AddListener(Close);
    }

    /// <summary>
    /// 장비 선택용 인벤토리 창을 초기화.
    /// </summary>
    /// <param name="equipmentSlot">플레이어가 누른 장비 슬롯.</param>
    /// <param name="equipmentType">장비 슬롯의 장비 타입.</param>
    public void LateInit(UI_HeroEquipmentSlot equipmentSlot, EquipmentType equipmentType)
    {
        UI_Inventory inventory = GetGameObject(GameObjects.Inventory).GetOrAddComponent<UI_Inventory>();
        inventory.LateInit(OnSlotSelected);
        _inventory = inventory;

        _bindingEquipmentSlot = equipmentSlot;

        inventory.HardClear();
        // 장비 해제용 커스텀 슬롯을 인벤토리 첫 슬롯 위치에 추가.
        EquipmentUnequipSlotDesign equipmentUnequipSlotDesign = new();
        UI_ItemSlot slot = equipmentUnequipSlotDesign.CreateItemSlot(inventory.transform);
        inventory.AddSlot(slot);
        slot.OnClickAction += OnUnequipSelected;

        List<ItemInstanceData> items = Managers.InvMng.GetAllEquipmentOfType(equipmentType);
        foreach (var item in items)
        {
            inventory.AddItem(item);
        }

        ScrollRect scrollRect = GetComponentInChildren<ScrollRect>();
        scrollRect.verticalNormalizedPosition = 1f; // 스크롤을 맨 위로 이동
    }

    /// <summary>
    /// 장비 해제 커스텀 슬롯을 선택했을 때 호출되는 콜백 함수
    /// </summary>
    private void OnUnequipSelected(UI_ItemSlot selectedSlot)
    {
        _bindingEquipmentSlot.ChangeEquipment(null); // 장비 해제
        _inventory.ClearActionCallbackOnSlots();
        Close();
    }

    /// <summary>
    /// 유저가 _inventory에서 슬롯을 선택했을 때 호출되는 콜백 함수.
    /// </summary>
    private void OnSlotSelected(UI_ItemSlot selectedSlot)
    {
        if (selectedSlot.ItemInstanceData == null) return;

        if (selectedSlot.ItemInstanceData is not EquipmentInstanceData equipmentInstanceData) return;
        
        _bindingEquipmentSlot.ChangeEquipment(equipmentInstanceData);
        _inventory.ClearActionCallbackOnSlots();
        Close();
    }

    public override void Close()
    {
        _inventory.ClearActionCallbackOnSlots();
        base.Close();
    }

    private void OnDestroy()
    {
        _inventory.ClearActionCallbackOnSlots();
    }
}
