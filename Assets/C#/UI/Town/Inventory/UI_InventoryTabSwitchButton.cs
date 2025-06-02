using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 인벤토리 탭간 전환 버튼: TownInventory, TownStore에서 사용
/// </summary>
public class UI_InventoryTabSwitchButton : MonoBehaviour
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
        _button.interactable = false;
        _image.sprite = sp_active;
        _text.color = c_text_active;
    }

    public void SetInactive()
    {
        _button.interactable = true;
        _image.sprite = sp_inactive;
        _text.color = c_text_inactive;
    }

    public void AddListener(UnityEngine.Events.UnityAction action)
    {
        _button.onClick.AddListener(action);
    }
}
