using DG.Tweening;
using System;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

/// <summary>
/// 인벤토리 등에서 아이템의 상세 정보를 보여주는 팝업 UI.
/// </summary>
public class UI_ItemDetailPopup : UI_Popup
{
    enum Images
    {
        Image_ItemImage,
        Image_ItemTypeIcon,

        Image_Hero,
    }

    enum Texts
    {
        Text_ItemName,
        Text_ItemQuantity,
        Text_ItemDescription,

        Text_EquippedHero,
    }

    enum GameObjects
    {
        EquippedHeroFrame
    }

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));
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
    public void DisableEquippedHeroInfo() => GetGameObject(GameObjects.EquippedHeroFrame).SetActive(false);
    public void SetEquippedHeroInfo(int heroInstanceId)
    {
        HeroInstanceData heroInstanceData = Managers.HeroMng.HeroStorage.GetHeroInstanceData(heroInstanceId);

        GetGameObject(GameObjects.EquippedHeroFrame).SetActive(true);
        GetImage(Images.Image_Hero).sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.CREATURE_IMAGE_PATH_PREFIX + heroInstanceData.ClassName +"_front");
        GetText(Texts.Text_EquippedHero).text = "Equipped By " + heroInstanceData.CustomName;
    }
}
