using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private UI_HeroEquipmentSlot _bindedEquipmentSlot;

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

        _bindedEquipmentSlot = equipmentSlot;

        List<InventoryEntry> items = Managers.InvMng.GetAllEquipmentOfType(equipmentType);
        foreach (var item in items)
        {
            inventory.AddItem(item.ItemData, item.Quantity);
        }
    }

    private void OnEquipmentSelected(UI_InventorySlot selectedSlot)
    {
        _bindedEquipmentSlot.OnEquipmentSelected(selectedSlot);
        _inventory.RemoveActionCallbackOnSlots(OnEquipmentSelected);
        Close();
    }

    public override void Close()
    {
        _inventory.RemoveActionCallbackOnSlots(OnEquipmentSelected);
        base.Close();
    }

    private void OnDestroy()
    {
        _inventory.RemoveActionCallbackOnSlots(OnEquipmentSelected);
    }
}
