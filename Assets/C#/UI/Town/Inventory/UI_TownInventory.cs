using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TownInventory : UI_Base
{
    enum Buttons
    {
        Button_Close,
    }

    enum ButtonObjects
    {
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

    private InventoryTab _activeTab; // 현재 활성화중인 인벤토리 탭

    private Dictionary<InventoryTab, UI_InventoryTabSwitchButton> _tabToButton = new();
    private readonly Dictionary<InventoryTab, ItemType> _tabToItemType = new()
    {
        { InventoryTab.Tab_Weapons, ItemType.Weapon },
        { InventoryTab.Tab_Armors, ItemType.Armor },
        { InventoryTab.Tab_Consumables, ItemType.Consumable }
    };

    private RectTransform _rectTransform;
    private float _offscreenY; // 화면에서 UI를 숨길 때 이동할 Y좌표

    public override void Init() { }

    public void LateInit()
    {
        ShowInstantly();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(ButtonObjects));
        Bind<UI_Inventory>(typeof(InventoryTab));
        _rectTransform = GetComponent<RectTransform>();
        _offscreenY = _rectTransform.rect.height;

        GetButton(Buttons.Button_Close).onClick.AddListener(Close);

        foreach (InventoryTab tab in System.Enum.GetValues(typeof(InventoryTab)))
        {
            var buttonObj = GetGameObject((ButtonObjects)tab);
            var button = buttonObj.GetOrAddComponent<UI_InventoryTabSwitchButton>();
            button.Init();
            button.AddListener(() => ShowTab(tab));
            _tabToButton[tab] = button;
            button.SetInactive();
        }

        InitTab(InventoryTab.Tab_Weapons);
        InitTab(InventoryTab.Tab_Armors);
        InitTab(InventoryTab.Tab_Consumables);

        ShowTab(InventoryTab.Tab_Weapons);
        HideInstantly();
    }

    private void InitTab(InventoryTab tabName)
    {
        UI_Inventory inv = Get<UI_Inventory>(tabName);
        inv.LateInit();
        inv.HideInstantly();
    }

    private void ShowTab(InventoryTab tabName)
    {
        // 기존 탭 숨김
        HideTab(_activeTab);

        var inventory = Get<UI_Inventory>(tabName);
        inventory.ShowInstantly();
        inventory.Clear();

        // 해당 탭에 맞는 아이템을 추가
        ItemType itemType = _tabToItemType[tabName];
        List<ItemInstanceData> items = Managers.InvMng.GetAllItemsOfType(itemType);
        foreach (var item in items)
        {
            inventory.AddItem(item);
        }

        // 현재 탭 버튼을 활성화
        _tabToButton[tabName].SetActive();
        _activeTab = tabName;
    }

    private void HideTab(InventoryTab tabName)
    {
        Get<UI_Inventory>(tabName).Clear();
        Get<UI_Inventory>(tabName).HideInstantly();
        _tabToButton[tabName].SetInactive();
    }

    public override Tween Show()
    {
        gameObject.SetActive(true);
        ShowTab(InventoryTab.Tab_Weapons);
        return _rectTransform.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutCubic).OnComplete(() => { GetButton(Buttons.Button_Close).interactable = true; });
    }

    public override Tween Hide()
    {
        GetButton(Buttons.Button_Close).interactable = false;
        return _rectTransform.DOAnchorPosY(_offscreenY, 0.5f)
            .SetEase(Ease.InCubic)
            .OnComplete(() => gameObject.SetActive(false));
    }

    public override void HideInstantly()
    {
        _rectTransform.DOAnchorPosY(_offscreenY, 0f);
        base.HideInstantly();
    }

    public void Close()
    {
        Managers.TownMng.UI.CurrentOpenUI = null;
        Hide();
    }
}
