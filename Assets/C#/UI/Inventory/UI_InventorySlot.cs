using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;


public class UI_InventorySlot : UI_Base
{
    enum Texts
    {
        Text_Quantity
    }

    enum Images
    {
        Image_Item
    }

    enum GameObjects
    {
        Detail,
        Quantity
    }

    public ItemData ItemData { get; private set; }
    private int _quantity;
    public int Quantity { 
        get => _quantity; 
        set 
        {
            if (_quantity != 0 && value == 0)
            {
                UnbindItem();
            } else
            {
                _quantityText.text = value.ToString();
            }
            _quantity = value;
        } 
    }
    public bool IsEmpty => ItemData == null;

    private TextMeshProUGUI _quantityText;
    private Image _image;
    private GameObject _detailParent;
    private GameObject _quantityParent;

    public override void Init() {}

    public void LateInit()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));

        _quantityText = GetText(Texts.Text_Quantity);
        _image = GetImage(Images.Image_Item);
        _detailParent = GetGameObject(GameObjects.Detail);
        _quantityParent = GetGameObject(GameObjects.Quantity);
        HideDetail();
    }

    /// <summary>
    /// 아이템 데이터와 개수를 UI에 바인딩.
    /// </summary>
    public void BindItem(ItemData itemdata, int quantity)
    {
        ItemData = itemdata;
        Quantity = quantity;
        _image.sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.ITEMIMAGE_PATH_PREFIX + itemdata.ImagePath);
        _detailParent.SetActive(true);
        if (itemdata.ItemType == ItemType.Consumable) // 개수 표시는 소모품에만
        {
            _quantityParent.SetActive(true);
        }
        else
        {
            _quantityParent.SetActive(false);
        }
    }

    public void UnbindItem()
    {
        ItemData = null;
        _quantity = 0;
        HideDetail();
    }

    public void HideDetail()
    {
        _detailParent.SetActive(false);
    }
}