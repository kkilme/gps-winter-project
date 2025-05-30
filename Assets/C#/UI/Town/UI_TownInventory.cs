using DG.Tweening;
using System.Collections.Generic;
using TMPro;
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

    /// <summary>
    /// 인벤토리 탭간 전환 버튼
    /// </summary>
    private class UI_TownInventoryButton: MonoBehaviour
    {
        private Image _image;
        private TextMeshProUGUI _text;
        private Button _button;

        private static Sprite sp_inactive;
        private static Sprite sp_active;
        private readonly static Color c_text_inactive = new Color(135f / 255f, 120f / 255f, 98f / 255f);
        private readonly static Color c_text_active = new Color(226f / 255f, 199f / 255f, 153f / 255f);

        public void Init()
        {
            _image = GetComponent<Image>();
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _button = GetComponent<Button>();
            sp_inactive = sp_inactive != null ? sp_inactive : Managers.ResourceMng.Load<Sprite>(GlobalValues.TOWNTEXTURE_PATH_PREFIX + "buttonframe_inactive");
            sp_active = sp_active != null ? sp_active : Managers.ResourceMng.Load<Sprite>(GlobalValues.TOWNTEXTURE_PATH_PREFIX + "buttonframe_active");
        }

        public void SetActive()
        {
            _image.sprite = sp_active;
            _text.color = c_text_active;
        }

        public void SetInactive()
        {
            _image.sprite = sp_inactive;
            _text.color = c_text_inactive;
        }

        public void AddListener(UnityEngine.Events.UnityAction action)
        {
            _button.onClick.AddListener(action);
        }
    }

    private InventoryTab _activeTab;

    private Dictionary<InventoryTab, UI_TownInventoryButton> _tabToButton = new();
    private readonly Dictionary<InventoryTab, ItemType> _tabToItemType = new()
    {
        { InventoryTab.Tab_Weapons, ItemType.Weapon },
        { InventoryTab.Tab_Armors, ItemType.Armor },
        { InventoryTab.Tab_Consumables, ItemType.Consumable }
    };

    private RectTransform _rectTransform;
    private float _offscreenY; // 화면에서 UI를 숨길 때 이동할 Y좌표

    private ScrollRect _scrollRect;

    public override void Init() {}

    public void LateInit()
    {
        ShowInstantly();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(ButtonObjects));
        Bind<UI_Inventory>(typeof(InventoryTab));
        _rectTransform = GetComponent<RectTransform>();
        _offscreenY = _rectTransform.rect.height;
        _scrollRect = GetComponentInChildren<ScrollRect>();

        GetButton(Buttons.Button_Close).onClick.AddListener(Close);

        foreach (InventoryTab tab in System.Enum.GetValues(typeof(InventoryTab)))
        {
            var buttonObj = GetGameObject((ButtonObjects)tab);
            var button = buttonObj.GetOrAddComponent<UI_TownInventoryButton>();
            button.Init();
            button.AddListener(() => ShowInventoryTab(tab));
            _tabToButton[tab] = button;
            button.SetInactive();
        }

        InitInventoryTab(InventoryTab.Tab_Weapons);
        InitInventoryTab(InventoryTab.Tab_Armors);
        InitInventoryTab(InventoryTab.Tab_Consumables);

        ShowInventoryTab(InventoryTab.Tab_Weapons);
        HideInstantly();
    }

    private void InitInventoryTab(InventoryTab inventoryTab)
    {
        UI_Inventory inv = Get<UI_Inventory>(inventoryTab);

        // ui의 각종 초기 값이 세팅되도록 활성화 -> 초기화 -> 비활성화 과정을 거침
        inv.ShowInstantly();
        inv.LateInit();
        inv.HideInstantly();
    }

    private void ShowInventoryTab(InventoryTab inventoryTab)
    {
        // 기존 탭 숨김
        HideInventoryTab(_activeTab);

        var inventory = Get<UI_Inventory>(inventoryTab);
        inventory.ShowInstantly();
        inventory.Clear();

        // 해당 탭에 맞는 아이템을 추가
        ItemType itemType = _tabToItemType[inventoryTab];
        List<ItemInstanceData> items = Managers.InvMng.GetAllItemsOfType(itemType);
        foreach (var item in items)
        {
            inventory.AddItemSlot(item);
        }

        // ScrollRect의 컨텐츠를 현재 인벤토리로 설정
        RectTransform rt = inventory.gameObject.transform as RectTransform;
        rt.localPosition = new Vector2(0, 0);
        _scrollRect.content = rt;

        // 현재 탭 버튼을 활성화
        _tabToButton[inventoryTab].SetActive();
        _activeTab = inventoryTab;

        // 스크롤 위치를 맨 위로 초기화
         _scrollRect.verticalNormalizedPosition = 1f;
    }

    private void HideInventoryTab(InventoryTab inventoryTab)
    {
        Get<UI_Inventory>(inventoryTab).HideInstantly();
        _tabToButton[inventoryTab].SetInactive();
    }

    public override Tween Show()
    {
        gameObject.SetActive(true);
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
