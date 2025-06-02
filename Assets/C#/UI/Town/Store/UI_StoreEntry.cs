using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UI_StoreEntry : UI_Base
{
    enum GameObjects
    {
        Stock,
        Design_SoldOut,
    }

    enum Texts
    {
        Text_ItemName,
        Text_Price
    }

    enum Images
    {
        Image_Item,
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
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        Get<GameObject>(GameObjects.Design_SoldOut).SetActive(false);

        if (Managers.DataMng.ItemDataDict.TryGetValue(entryData.ItemDataId, out ItemData itemData))
        {
            Get<TextMeshProUGUI>(Texts.Text_ItemName).text = itemData.Name;
            Get<TextMeshProUGUI>(Texts.Text_Price).text = entryData.Price.ToString();
            Get<Image>(Images.Image_Item).sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.ITEMIMAGE_PATH_PREFIX + itemData.ImagePath);
            SetStock(entryData.Stock);
        } else
        {
            Debug.LogError($"[UI_StoreEntry] ItemData with ID {entryData.ItemDataId} not found.");
        }
    }

    public void SetStock(int stock)
    {
        if (stock == -1) // -1: 무제한 재고
        {
            Get<GameObject>(GameObjects.Design_SoldOut).SetActive(false);
            Get<GameObject>(GameObjects.Stock).SetActive(false);
            Get<Button>(Buttons.Button_Buy).interactable = true;
        } else if (stock == 0)
        {
            Get<GameObject>(GameObjects.Stock).SetActive(true);
            Get<GameObject>(GameObjects.Design_SoldOut).SetActive(true);
            Get<GameObject>(GameObjects.Stock).GetComponentInChildren<TextMeshProUGUI>().text = "Stock: 0";
            Get<Button>(Buttons.Button_Buy).interactable = false;
        } else
        {
            Get<GameObject>(GameObjects.Stock).SetActive(true);
            Get<GameObject>(GameObjects.Design_SoldOut).SetActive(false);
            Get<GameObject>(GameObjects.Stock).GetComponentInChildren<TextMeshProUGUI>().text = "Stock: " + stock.ToString();
            Get<Button>(Buttons.Button_Buy).interactable = true;
        }

    }
}
