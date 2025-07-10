using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UI_AreaEncounterPopup : UI_Popup
{
    enum Texts
    {
        Text_EncounterName,
        Text_EncounterDescription,
    }

    enum Images
    {
        Image_EncounterImage,
    }

    enum GameObjects
    {
        ResultDetails
    }

    enum Buttons
    {
        Button_Try,
        Button_Leave,
    }

    private UI_CoinTossDisplay _coinTossDisplay;
    private Transform _resultDetailParent;

    private Dictionary<int, UI_EncounterResultLine> _resultDetails = new(); // 코인토스 성공 개수에 따른 result line을 저장해둠. 결과에 따라 line을 하이라이트

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        _coinTossDisplay = GetComponentInChildren<UI_CoinTossDisplay>();

        _resultDetailParent = GetGameObject(GameObjects.ResultDetails).transform;
        for(int i = _resultDetailParent.childCount - 1; i >= 0; i--)
        {
            Destroy(_resultDetailParent.GetChild(i).gameObject); // 에디터에서의 디스플레이용으로 추가해놓은 기존 자식 오브젝트 제거
        }
    }

    /// <summary>
    /// encounterData를 UI에 바인딩하여 관련 UI 요소 업데이트.
    /// </summary>
    public void BindEncounterData(EncounterData encounterData, int coinCount, StatName usingStat)
    {
        Get<TextMeshProUGUI>(Texts.Text_EncounterName).text = encounterData.Name;
        Get<TextMeshProUGUI>(Texts.Text_EncounterDescription).text = encounterData.Description;
        Get<Image>(Images.Image_EncounterImage).sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.AREAENCOUNTER_PATH_PREFIX + encounterData.ImagePath);
        _coinTossDisplay.SetData(coinCount, usingStat);
    }

    /// <summary>
    /// Try, Leave버튼에 적절히 콜백 등록
    /// </summary>
    public void SetupButtons(bool isLeavable, Action onTry, Action onLeave)
    {
        var tryButton = GetButton(Buttons.Button_Try);
        tryButton.onClick.RemoveAllListeners(); // 중복 방지
        tryButton.onClick.AddListener(() => {
            tryButton.interactable = false;
            onTry?.Invoke();
        });

        var leaveButton = GetButton(Buttons.Button_Leave);
        if (!isLeavable)
        {
            leaveButton.gameObject.SetActive(false);
        }
        else
        {
            leaveButton.onClick.RemoveAllListeners();
            leaveButton.onClick.AddListener(() => {
                leaveButton.interactable = false;
                onLeave?.Invoke();
            });
        }
    }

    /// <summary>
    /// 코인 토스 결과에 따른 상세 결과를 알려주는 UI를 추가한다. 
    /// </summary>
    /// <param name="successCount">결과를 위해 성공해야하는 코인 수.</param>
    /// <param name="chanceOfResult">해당 결과가 나올 수 있는 확률(%)</param>
    /// <param name="resultDescription">해당 결과에 대한 설명</param>
    public void AddResultDetailLine(int successCount, int chanceOfResult, string resultDescription)
    {
        UI_EncounterResultLine line = Managers.UIMng.MakeSubItemUI<UI_EncounterResultLine>(_resultDetailParent, "Area/" + nameof(UI_EncounterResultLine));
        line.SetData(successCount.ToString(), chanceOfResult, resultDescription);
        _resultDetails.Add(successCount, line);
    }

    /// <summary>
    /// 코인 토스 결과에 따른 상세 결과를 알려주는 UI를 추가한다. 
    /// </summary>
    /// <param name="minSuccessCount">결과를 위해 성공해야하는 최소 코인 수.</param>
    /// <param name="maxSuccessCount">결과를 위해 성공해야하는 최대 코인 수.</param>
    /// <param name="chanceOfResult">해당 결과가 나올 수 있는 확률(%)</param>
    /// <param name="resultDescription">해당 결과에 대한 설명</param>
    public void AddResultDetailLine(int minSuccessCount, int maxSuccessCount, int chanceOfResult, string resultDescription)
    {
        UI_EncounterResultLine line = Managers.UIMng.MakeSubItemUI<UI_EncounterResultLine>(_resultDetailParent, "Area/" + nameof(UI_EncounterResultLine));
        line.SetData($"{minSuccessCount}-{maxSuccessCount}", chanceOfResult, resultDescription);
        for (int i = minSuccessCount; i <= maxSuccessCount; i++)
        {
            _resultDetails.Add(i, line);
        }
    }

    public override Tween Show()
    {
        gameObject.SetActive(true);
        return Panel.DOScale(new Vector3(0.2f, 0.2f, 1f), 0.4f).From().SetEase(Ease.InQuad);
    }

    /// <summary>
    /// CoinTossDisplay UI를 통해 코인 토스 결과를 표시한다.
    /// </summary>
    public IEnumerator ShowCoinTossResult(CoinTossHelper.CoinTossResult coinTossResult, StatName usingStat)
    {
        yield return _coinTossDisplay.ShowResult(coinTossResult.result, usingStat);
    }

    /// <summary>
    /// CoinCount에 해당하는 ResultLine UI를 하이라이트한다.
    /// </summary>
    public void HighlightResultLine(int coinCount)
    {
        _resultDetails[coinCount].Highlight();
    }
}
