using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Town 상점의 한 아이템을 나타내는 UI.
/// </summary>
public class UI_StoreEntry : UI_Base
{
    enum GameObjects
    {
        Design_SoldOut,
    }

    enum Texts
    {
        Text_ItemName,
        Text_Price,
        Text_Stock,
    }

    enum ItemSlot
    {
        UI_ItemSlot,
    }

    enum Buttons
    {
        Button_Buy
    }

    private StoreEntryData _entryData;

    public override void Init() { }

    public void LateInit(StoreEntryData entryData)
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<UI_ItemSlot>(typeof(ItemSlot));
        Bind<Button>(typeof(Buttons));

        Get<GameObject>(GameObjects.Design_SoldOut).SetActive(false);

        if (Managers.DataMng.ItemDataDict.TryGetValue(entryData.ItemDataId, out ItemData itemData))
        {
            _entryData = entryData;
            Get<TextMeshProUGUI>(Texts.Text_ItemName).text = itemData.Name;
            Get<TextMeshProUGUI>(Texts.Text_Price).text = entryData.Price.ToString();
            Get<UI_ItemSlot>(ItemSlot.UI_ItemSlot).LateInit(new DefaultItemSlotDesign());
            Get<UI_ItemSlot>(ItemSlot.UI_ItemSlot).BindItem(itemData);
            Get<Button>(Buttons.Button_Buy).onClick.AddListener(ShowBuyConfirmPopup);

            UpdateStock(entryData);
        } else
        {
            Debug.LogError($"[UI_StoreEntry] ItemData with ID {entryData.ItemDataId} not found.");
        }
    }

    public void UpdateStock(StoreEntryData entryData)
    {
        if (!entryData.HasStockLimit) // 무제한 재고
        {
            Get<GameObject>(GameObjects.Design_SoldOut).SetActive(false);
            GetText(Texts.Text_Stock).text = "Stock: ∞";
            Get<Button>(Buttons.Button_Buy).interactable = true;
        } else if (entryData.Stock == 0)
        {
            Get<GameObject>(GameObjects.Design_SoldOut).SetActive(true);
            GetText(Texts.Text_Stock).text = "Stock: 0";
            Get<Button>(Buttons.Button_Buy).interactable = false;
        } else
        {
            Get<GameObject>(GameObjects.Design_SoldOut).SetActive(false);
            GetText(Texts.Text_Stock).text = "Stock: " + entryData.Stock.ToString();
            Get<Button>(Buttons.Button_Buy).interactable = true;
        }

    }

    private void ShowBuyConfirmPopup()
    {
        if (_entryData.HasStockLimit && _entryData.Stock <= 0) return;
        var popup = Managers.UIMng.ShowPopupUI<UI_BuyConfirmPopup>();
        popup.LateInit(_entryData, this);
    }
}
