using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Town Store에서 구매 패널을 나타냄.
/// </summary>
public class UI_TownStore_BuyPanel : UI_Base, IStorePanel
{
    enum ButtonObjects
    {
        Button_Weapons,
        Button_Armors,
        Button_Consumables,
    }

    enum BuyPanelTab
    {
        Tab_Weapons,
        Tab_Armors,
        Tab_Consumables,
    }

    private BuyPanelTab _activeTab;

    private Dictionary<BuyPanelTab, UI_InventoryTabSwitchButton> _tabToButton = new();
    private readonly Dictionary<BuyPanelTab, ItemType> _tabToItemType = new()
    {
        { BuyPanelTab.Tab_Weapons, ItemType.Weapon },
        { BuyPanelTab.Tab_Armors, ItemType.Armor },
        { BuyPanelTab.Tab_Consumables, ItemType.Consumable }
    };

    private ScrollRect _scrollRect;

    public override void Init() { }

    public void LateInit()
    {
        base.ShowInstantly();
        Bind<GameObject>(typeof(ButtonObjects));
        Bind<UI_TownStore_BuyTab>(typeof(BuyPanelTab));
        _scrollRect = GetComponentInChildren<ScrollRect>();

        foreach (BuyPanelTab tab in System.Enum.GetValues(typeof(BuyPanelTab)))
        {
            var buttonObj = GetGameObject((ButtonObjects)tab);
            var button = buttonObj.GetOrAddComponent<UI_InventoryTabSwitchButton>();
            button.Init();
            button.AddListener(() => ShowTab(tab));
            _tabToButton[tab] = button;
            button.SetInactive();
        }

        HideInstantly();
    }

    private void ShowTab(BuyPanelTab tabName)
    {
        // 기존 탭 숨김
        HideTab(_activeTab);

        var tab = Get<UI_TownStore_BuyTab>(tabName);
        tab.ShowInstantly(_tabToItemType[tabName]);

        // ScrollRect의 컨텐츠를 현재 탭으로 설정
        RectTransform rt = tab.gameObject.transform as RectTransform;
        rt.localPosition = new Vector2(0, 0);
        _scrollRect.content = rt;

        // 현재 탭 버튼을 활성화
        _tabToButton[tabName].SetActive();
        _activeTab = tabName;

        // 스크롤 위치를 맨 위로 초기화
        _scrollRect.verticalNormalizedPosition = 1f;
    }

    private void HideTab(BuyPanelTab tabName)
    {
        Get<UI_TownStore_BuyTab>(tabName).HideInstantly();
        _tabToButton[tabName].SetInactive();
    }

    public override void ShowInstantly()
    {
        base.ShowInstantly();
        ShowTab(BuyPanelTab.Tab_Weapons);
    }

}
