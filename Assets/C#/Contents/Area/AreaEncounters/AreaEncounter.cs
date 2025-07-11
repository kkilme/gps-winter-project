using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;


public abstract class AreaEncounter
{
    protected EncounterData _data;
    protected int _coinCount;
    protected StatName _usingStat;

    protected UI_AreaEncounterPopup _popup;

    /// <summary>
    /// 데이터 바인딩 및 coinCount와 usingStat을 초기화. ObjectHolder에서 Encounter 생성과 동시에 호출한다.
    /// </summary>
    public abstract void SetData(EncounterData encounterData);

    /// <summary>
    /// Encounter 팝업 생성 및 초기화 로직
    /// </summary>
    public virtual void ShowEncounterPopup()
    {
        _popup = Managers.UIMng.ShowPopupUI<UI_AreaEncounterPopup>();
        _popup.BindEncounterData(_data, _coinCount, _usingStat);
        _popup.SetupButtons(
            _data.IsLeavable,
            onTry: () => CoroutineRunner.Instance.StartCoroutine(TryEncounter()),
            onLeave: EndEncounter
        );
        AddResultLines();
        _popup.Show();
    }

    /// <summary>
    /// 코인 토스 결과에 따른 Encounter 결과를 알려주는 UI_EncounterResultLine를 popup에 추가.
    /// </summary>
    protected abstract void AddResultLines();

    /// <summary>
    /// Popup의 Try 버튼을 선택 시 수행되는 로직
    /// </summary>
    private IEnumerator TryEncounter()
    {
        int statValue = Managers.HeroMng.HeroParty.GetAverageStat(_usingStat);
        CoinTossHelper.CoinTossResult result = CoinTossHelper.CoinToss(_coinCount, statValue);

        yield return _popup.ShowCoinTossResult(result, _usingStat);

        yield return new WaitForSeconds(1f);

        _popup.HighlightResultLine(result.successCount);
        ExecuteResult(result.successCount);

        yield return new WaitForSeconds(1f);

        EndEncounter();
    }

    /// <summary>
    /// 코인 토스 결과에 따른 실제 Encounter 결과 로직 수행
    /// </summary>
    protected abstract void ExecuteResult(int coinSuccessCount);

    /// <summary>
    /// Encounter가 종료될 시 수행되는 로직
    /// </summary>
    protected virtual void EndEncounter()
    {
        _popup.Close();
        CoroutineRunner.Instance.StartCoroutine(Managers.AreaMng.OnTileEventFinish());
    }
}
