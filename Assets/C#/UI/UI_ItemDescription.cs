using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemDescription : UI_Base
{
    enum Images
    {
        Icon
    }

    enum Texts
    {
        Name,
        Description
    }
    
    enum GameObjects
    {
        Group
    }
    
    public override void Init()
    {
        Bind<UnityEngine.UI.Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
    }
    
    public void SetInfo(BaseItem item)
    {
        //GetImage(Images.Icon).sprite = item.image;
        GetText(Texts.Name).text = item.ItemData.Name;
        GetText(Texts.Description).text = item.ItemData.Description;
    }

    public void OnMouseEnter(PointerEventData data)
    {
        ChangeImageVisibility(true);
    }

    public void OnMouseExit(PointerEventData data)
    {
        ChangeImageVisibility(false);
    }

    public void ChangeImageVisibility(bool visible)
    {
        GetGameObject(GameObjects.Group).SetActive(visible);
        GetComponent<UnityEngine.UI.Image>().enabled = visible;
    }
}
