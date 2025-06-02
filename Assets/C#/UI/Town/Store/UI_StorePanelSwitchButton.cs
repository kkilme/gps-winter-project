using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 상점 패널간 전환 버튼: TownStore에서 사용
/// </summary>
public class UI_StorePanelSwitchButton : MonoBehaviour
{
    private Image _image;
    private TextMeshProUGUI _text;
    private Button _button;

    private readonly static Color c_image_inactive = new Color(.2f, .18f, .15f);
    private readonly static Color c_image_active = new Color(.3f, .28f, .25f);
    private readonly static Color c_text_inactive = new Color(135f / 255f, 120f / 255f, 98f / 255f);
    private readonly static Color c_text_active = new Color(226f / 255f, 199f / 255f, 153f / 255f);

    public void Init()
    {
        _image = GetComponent<Image>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _button = GetComponent<Button>();
    }

    public void SetActive()
    {
        _button.interactable = false;
        _image.color = c_image_active;
        _text.color = c_text_active;
    }

    public void SetInactive()
    {
        _button.interactable = true;
        _image.color = c_image_inactive;
        _text.color = c_text_inactive;
    }

    public void AddListener(UnityEngine.Events.UnityAction action)
    {
        _button.onClick.AddListener(action);
    }
}
