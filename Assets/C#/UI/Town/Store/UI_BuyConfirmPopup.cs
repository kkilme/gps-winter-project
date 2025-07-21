using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UI_BuyConfirmPopup : UI_Popup
{
    enum Texts
    {
        Text_ItemName,
        Text_Stock,
        Text_Price,
        Text_CurrentGold,
    }

    enum Buttons
    {
        Button_Close,

        Button_Buy,
        
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

    private UI_StoreEntry ui_StoreEntry; // 구매할 아이템의 UI_StoreEntry
    private StoreEntryData _storeEntryData; // 구매할 아이템의 데이터
    private int _quantity = 1; // 구매할 아이템의 수량

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<UI_ItemSlot>(typeof(ItemSlot));
        Bind<TMP_InputField>(typeof(InputField));

        Get<UI_ItemSlot>(ItemSlot.UI_ItemSlot).LateInit(new DefaultItemSlotDesign());

        Get<Button>(Buttons.Button_Close).onClick.AddListener(Close);

        Get<Button>(Buttons.Button_MultipleMinus).onClick.AddListener(() => AddQuantity(-5));
        Get<Button>(Buttons.Button_SingleMinus).onClick.AddListener(() => AddQuantity(-1));
        Get<Button>(Buttons.Button_SinglePlus).onClick.AddListener(() => AddQuantity(1));
        Get<Button>(Buttons.Button_MultiplePlus).onClick.AddListener(() => AddQuantity(5));

        Get<Button>(Buttons.Button_Buy).onClick.AddListener(Buy);
    }

    public void LateInit(StoreEntryData storeEntryData, UI_StoreEntry ui_StoreEntry)
    {
        _storeEntryData = storeEntryData;
        this.ui_StoreEntry = ui_StoreEntry;

        ItemData itemData = Managers.DataMng.ItemDataDict[storeEntryData.ItemDataId];
        GetText(Texts.Text_ItemName).text = itemData.Name;
        Get<UI_ItemSlot>(ItemSlot.UI_ItemSlot).BindItem(itemData);

        if(storeEntryData.HasStockLimit) GetText(Texts.Text_Stock).text = "Stock: " + storeEntryData.Stock.ToString("N0");
        else GetText(Texts.Text_Stock).text = "Stock: ∞";
        GetText(Texts.Text_CurrentGold).text = Managers.InvMng.Gold.ToString("N0");

        Get<TMP_InputField>(InputField.InputField_Quantity).onEndEdit.AddListener(OnQuantityEdited);
        UpdatePrice();
    }

    private void AddQuantity(int amount)
    {
        SetQuantity(_quantity + amount);
    }

    private void SetQuantity(int quantity)
    {
        quantity = Mathf.Clamp(quantity, 1, 99); // 한번에 구매할 수 있는 수량은 최소 1, 최대 99개
        if (_storeEntryData.HasStockLimit) quantity = Mathf.Min(quantity, _storeEntryData.Stock);
        _quantity = quantity;

        Get<TMP_InputField>(InputField.InputField_Quantity).text = _quantity.ToString();
        UpdatePrice();
    }

    private void OnQuantityEdited(string quantity)
    {
        if(int.TryParse(quantity, out int parsedQuantity) && parsedQuantity > 0)
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
        if(_storeEntryData == null)
        {
            Debug.LogError("[UI_BuyConfirmPopup] Store entry data is not set.");
            return;
        }

        GetText(Texts.Text_Price).text = (_storeEntryData.Price * _quantity).ToString("N0");
        if(Managers.InvMng.Gold < _storeEntryData.Price * _quantity)
        {
            GetText(Texts.Text_Price).color = Color.red; // 가격이 부족할 경우 빨간색으로 표시
        }
        else
        {
            GetText(Texts.Text_Price).color = Color.white;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetText(Texts.Text_Price).GetComponentInParent<RectTransform>()); // Preferred Width에 맞추기 위해 Horizontal Layout 강제 업데이트
    }

    private void Buy()
    {
        if(_storeEntryData == null)
        {
            Debug.LogError("[UI_BuyConfirmPopup] Store entry data is not set.");
            return;
        }

        int price = _storeEntryData.Price * _quantity;
        if (Managers.InvMng.Gold < price)
        {
            // TODO?: UI로 경고 표시
            return;
        }

        Managers.SoundMng.PlayGoldSound();
        Managers.InvMng.RemoveGold(price); // 골드 차감
        Managers.InvMng.AddItem(_storeEntryData.ItemDataId, _quantity); // 아이템 추가
        _storeEntryData.Stock -= _quantity; // 재고 감소
        ui_StoreEntry.UpdateStock(_storeEntryData); // UI 업데이트
        Close(); // 팝업 닫기
    }
}
