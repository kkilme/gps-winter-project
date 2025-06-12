using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// TownStore의 SellPanel에서 아이템 판매를 확인하는 패널 UI. 개수 설정 및 판매 버튼을 포함.
/// </summary>
public class UI_SellConfirmPanel : UI_Base
{
    enum Texts
    {
        Text_ItemName,
        Text_Stock,
        Text_Price,
    }

    enum Buttons
    {
        Button_Close,

        Button_Sell,

        Button_MultipleMinus,
        Button_SingleMinus,
        Button_SinglePlus,
        Button_MultiplePlus
    }

    enum ItemSlot
    {
        UI_ItemSlot
    }

    enum InputField
    {
        InputField_Quantity
    }

    public Action OnSell;
    private ItemInstanceData _itemInstanceData; // 판매할 아이템의 데이터
    private int _quantity = 1; // 판매할 아이템의 수량

    private RectTransform _rect;

    public override void Init(){}

    public void LateInit()
    {
        ShowInstantly();

        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<UI_ItemSlot>(typeof(ItemSlot));
        Bind<TMP_InputField>(typeof(InputField));
        _rect = GetComponent<RectTransform>();

        Get<Button>(Buttons.Button_Close).onClick.AddListener(HideInstantly);

        Get<Button>(Buttons.Button_MultipleMinus).onClick.AddListener(() => AddQuantity(-5));
        Get<Button>(Buttons.Button_SingleMinus).onClick.AddListener(() => AddQuantity(-1));
        Get<Button>(Buttons.Button_SinglePlus).onClick.AddListener(() => AddQuantity(1));
        Get<Button>(Buttons.Button_MultiplePlus).onClick.AddListener(() => AddQuantity(5));

        Get<Button>(Buttons.Button_Sell).onClick.AddListener(() => Sell());

        Get<TMP_InputField>(InputField.InputField_Quantity).onEndEdit.AddListener(OnQuantityEdited);

        HideInstantly();
    }

    public void BindItem(ItemInstanceData itemInstanceData)
    {
        _itemInstanceData = itemInstanceData;
        SetQuantity(1);
        GetText(Texts.Text_ItemName).text = itemInstanceData.ItemData.Name;
        Get<UI_ItemSlot>(ItemSlot.UI_ItemSlot).BindItem(itemInstanceData.ItemData);
        GetText(Texts.Text_Stock).text = "Stock: " + itemInstanceData.Quantity.ToString("N0");

        UpdatePrice();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rect); // Layout Group 강제 업데이트
    }

    private void AddQuantity(int amount)
    {
        SetQuantity(_quantity + amount);
    }

    private void SetQuantity(int quantity)
    {
        quantity = Mathf.Clamp(quantity, 1, 999);
        quantity = Mathf.Min(quantity, _itemInstanceData.Quantity); // 판매할 수 있는 최대 수량은 아이템 인스턴스의 수량
        _quantity = quantity;

        Get<TMP_InputField>(InputField.InputField_Quantity).text = _quantity.ToString();
        UpdatePrice();
    }

    private void OnQuantityEdited(string quantity)
    {
        if (int.TryParse(quantity, out int parsedQuantity) && parsedQuantity > 0)
        {
            SetQuantity(parsedQuantity);
        }
        else
        {
            SetQuantity(1); // 잘못된 입력일 경우 기본값 1로 설정
        }
    }

    private void UpdatePrice()
    {
        if (_itemInstanceData == null)
        {
            Debug.LogError("[UI_SellConfirmPanel] Item Instance Data is not set.");
            return;
        }

        GetText(Texts.Text_Price).text = (_itemInstanceData.ItemData.SellPrice * _quantity).ToString("N0");
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetText(Texts.Text_Price).GetComponentInParent<RectTransform>()); // Preferred Width에 맞추기 위해 Horizontal Layout 강제 업데이트
    }

    private void Sell()
    {
        bool success = Managers.InvMng.SellItem(_itemInstanceData, _quantity);
        if (success)
        {
            OnSell?.Invoke();
        }
        HideInstantly();
    }
}
