using DG.Tweening;
using System;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;


public class UI_ItemDetail : UI_Popup
{
    enum Images
    {
        Image_ItemImage,
        Image_ItemTypeIcon,
    }

    enum Texts
    {
        Text_ItemName,
        Text_ItemQuantity,
        Text_ItemDescription,
    }

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
    }

    public void ApplyDesign(ItemData itemData, int quantity = 0)
    {
        var design = ItemDetailUIDesigner.GetDesign(itemData.ItemType);
        design.Apply(this, itemData, quantity);
    }

    public void ApplyGoldDesign(int goldAmount)
    {
        var design = ItemDetailUIDesigner.GetGoldDesign();
        design.Apply(this, null, goldAmount);
    }

    public void SetName(string name) => GetText(Texts.Text_ItemName).text = name;
    public void SetItemTypeIcon(string iconPath) => GetImage(Images.Image_ItemTypeIcon).sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.ITEMICON_PATH_PREFIX + iconPath);
    public void SetImage(string imagePath) => GetImage(Images.Image_ItemImage).sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.ITEMIMAGE_PATH_PREFIX + imagePath);
    public void SetDescription(string description) => GetText(Texts.Text_ItemDescription).text = description;
    public string GetDescription() => GetText(Texts.Text_ItemDescription).text;
    public void SetQuantity(int quantity)
    {
        if (quantity <= 0) GetText(Texts.Text_ItemQuantity).text = "";
        else GetText(Texts.Text_ItemQuantity).text = "x" + quantity.ToString();
    }
}
