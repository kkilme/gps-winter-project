using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TownInventory : UI_Base
{
    enum Buttons
    {
        Button_Close,

        Button_Weapons,
        Button_Armors,
        Button_Consumables,
    }

    enum InventoryTab
    {
        Tab_Weapons,
        Tab_Armors,
        Tab_Consumables,
    }

    private UI_Inventory _activeInventory;


    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<UI_Inventory>(typeof(InventoryTab));

        GetButton(Buttons.Button_Close).onClick.AddListener(Close);
    }

    public void InitInventory()
    {
        InitInventoryTab(Get<UI_Inventory>(InventoryTab.Tab_Weapons), ItemType.Weapon);
        InitInventoryTab(Get<UI_Inventory>(InventoryTab.Tab_Armors), ItemType.Armor);
        InitInventoryTab(Get<UI_Inventory>(InventoryTab.Tab_Consumables), ItemType.Consumable);
    }

    private void InitInventoryTab(UI_Inventory inventory, ItemType itemType)
    {
        inventory.SetSlotPath("Town/UI_InventorySlot_TownInventory");

        List<InventoryEntry> items = Managers.InvMng.GetAllItemsOfType(itemType);

        foreach (var item in items)
        {
            inventory.AddItem(item.ItemData, item.Quantity);
        }
    }

    public void Close()
    {
        Managers.TownMng.UI.CurrentOpenUI = null;
        Hide();
    }
}
