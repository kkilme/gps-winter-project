using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 어떠한 행동을 한번 더 확인시키는, 다양한 상황에서 사용될 수 있는 팝업 UI.
/// </summary>
public class UI_ConfirmActionPopup : UI_Popup
{
    enum Texts
    {
        Text_Title,
        Text_Confirm,
        Text_Cancel,
    }

    enum Buttons
    {
        Button_Confirm,
        Button_Cancel
    }

    private Action _confirmAction; // Confirm 버튼 클릭 시 실행될 액션

    public override void Init()
    {
        base.Init();
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        GetButton(Buttons.Button_Cancel).onClick.AddListener(Close);
        GetText(Texts.Text_Title).gameObject.SetActive(false);
    }

    public void SetConfirmAction(Action confirmAction)
    {
        _confirmAction = confirmAction;
        GetButton(Buttons.Button_Confirm).onClick.AddListener(() => 
        {
            _confirmAction?.Invoke();
            Close();
        });
    }

    public void SetTitle(string title)
    {
        GetText(Texts.Text_Title).gameObject.SetActive(true);
        GetText(Texts.Text_Title).text = title;
    }

    public void SetConfirmText(string confirmText)
    {
        GetText(Texts.Text_Confirm).text = confirmText;
    }

    public void SetCancelText(string cancelText)
    {
        GetText(Texts.Text_Cancel).text = cancelText;
    }
}
