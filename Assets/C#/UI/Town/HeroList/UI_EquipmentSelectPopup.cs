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

    private UI_Inventory _inventory;
    private UI_HeroEquipmentSlot _bindingEquipmentSlot;

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        GetButton(Buttons.Button_Close).onClick.AddListener(Close);
    }

    public void LateInit(UI_HeroEquipmentSlot equipmentSlot, EquipmentType equipmentType)
    {
        UI_Inventory inventory = GetGameObject(GameObjects.Inventory).GetOrAddComponent<UI_Inventory>();
        inventory.LateInit(OnSlotSelected);
        _inventory = inventory;

        _bindingEquipmentSlot = equipmentSlot;

        inventory.AddCustomSlot(OnUnequipSelected, Managers.ResourceMng.Load<Sprite>("Textures/PictoIcons/PictoIcon_X"));

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
    private void OnUnequipSelected(UI_InventorySlot selectedSlot)
    {
        _bindingEquipmentSlot.ChangeEquipment(null); // 장비 해제
        _inventory.ClearActionCallbackOnSlots();
        Close();
    }

    /// <summary>
    /// 유저가 Window에서 슬롯을 선택했을 때 호출되는 콜백 함수.
    /// </summary>
    private void OnSlotSelected(UI_InventorySlot selectedSlot)
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
