using DG.Tweening;
using TMPro;
using UnityEngine;


public class UI_CollapseInformer : UI_Base
{
    enum Texts
    {
        Text_Desc,
        Text_LeftTurn,
        Text_CollapseAmount,
    }

    private int _collapseTimer;

    private int _leftTurn;
    private RectTransform _leftTurnRect;
    private TextMeshProUGUI _leftTurnText;

    private static TMP_ColorGradient cg_Danger; // static으로 캐싱하여 한 번 로드 후 재사용
    private static TMP_ColorGradient cg_Safe;

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        _leftTurnText = Get<TextMeshProUGUI>(Texts.Text_LeftTurn);
        _leftTurnRect = _leftTurnText.GetComponent<RectTransform>();
        cg_Danger = Managers.ResourceMng.Load<TMP_ColorGradient>(GlobalValues.COLORGRADIENT_PATH_PREFIX + "CollpaseTimer_Danger");
        cg_Safe = Managers.ResourceMng.Load<TMP_ColorGradient>(GlobalValues.COLORGRADIENT_PATH_PREFIX + "CollpaseTimer_Safe");
    }

    public void BindData(int collapseTimer, int collapseAmount)
    {
        _collapseTimer = collapseTimer;
        _leftTurn = collapseTimer;
        _leftTurnText.text = _leftTurn.ToString();
        GetText(Texts.Text_CollapseAmount).text = collapseAmount.ToString();
    }

    public Tween ProgressTimer()
    {
        _leftTurn--;
        if (_leftTurn == 0)
        {
            _leftTurn = _collapseTimer;
        }

        if (_leftTurn <= 3)
        {
            _leftTurnText.colorGradientPreset = cg_Danger;
        }
        else
        {
            _leftTurnText.colorGradientPreset = cg_Safe;
        }

        _leftTurnText.text = _leftTurn.ToString();

        _leftTurnRect.localScale = _leftTurnRect.localScale * 2f;

        return _leftTurnRect.DOScale(Vector3.one, 0.5f);
    }

    public void OnCollapseFinished()
    {
        _leftTurnText.colorGradientPreset = cg_Safe;
        _leftTurnText.text = "-";
        GetText(Texts.Text_CollapseAmount).text = "-";
        GetText(Texts.Text_Desc).text = "Collapse Finished";
    }
}