using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 영웅 장비 장착 UI
/// </summary>
public class UI_EquipmentSelectWindow : UI_Popup
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
        inventory.LateInit(OnEquipmentSelected);
        _inventory = inventory;

        _bindingEquipmentSlot = equipmentSlot;

        List<ItemInstanceData> items = Managers.InvMng.GetAllEquipmentOfType(equipmentType);
        foreach (var item in items)
        {
            inventory.AddItem(item);
        }
    }

    /// <summary>
    /// 유저가 Window에서 장비를 선택했을 때 호출되는 콜백 함수.
    /// </summary>
    private void OnEquipmentSelected(UI_InventorySlot selectedSlot)
    {
        _bindingEquipmentSlot.ChangeEquipment(selectedSlot);
        _inventory.RemoveActionCallbackOnSlots(OnEquipmentSelected);
        Close();
    }

    public override void Close()
    {
        _bindingEquipmentSlot.IsSelectingEquipment = false;
        _inventory.RemoveActionCallbackOnSlots(OnEquipmentSelected);
        base.Close();
    }

    private void OnDestroy()
    {
        _bindingEquipmentSlot.IsSelectingEquipment = false;
        _inventory.RemoveActionCallbackOnSlots(OnEquipmentSelected);
    }
}
