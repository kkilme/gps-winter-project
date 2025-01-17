using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class UI_BattleScene : UI_Scene
{
	enum SubItemUI
	{
		UI_BattleActionPanel,
		UI_CoinToss,
		UI_TurnState,
        UI_BattleVictory
	}

	public UI_BattleActionPanel BattleActionPanel { get; protected set; }
	public UI_CoinToss CoinTossUI { get; protected set; }
	
    public override void Init()
    {
        base.Init();

		Bind<UI_Base>(typeof(SubItemUI));
        Managers.UIMng.ShowPlayerProfileGroupUI(true);
        BattleActionPanel = Get<UI_Base>(SubItemUI.UI_BattleActionPanel).GetOrAddComponent<UI_BattleActionPanel>();
		BattleActionPanel.gameObject.SetActive(false);
		CoinTossUI = Get<UI_Base>(SubItemUI.UI_CoinToss).GetOrAddComponent<UI_CoinToss>();
    }

    public void OnBattleEnd(Define.BattleResultType battleResult)
    {
        switch (battleResult)
        {
            case Define.BattleResultType.Victory:
                Get<UI_Base>(SubItemUI.UI_BattleActionPanel).gameObject.SetActive(false);
                Get<UI_Base>(SubItemUI.UI_CoinToss).gameObject.SetActive(false);

                // Turn 상태바 움직임을 통해 자연스럽게 숨기기
                Transform turnStateUI = Get<UI_Base>(SubItemUI.UI_TurnState).gameObject.transform;
                turnStateUI.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 90), 1f).OnComplete(() =>
                {
                    turnStateUI.gameObject.SetActive(false);
                    Get<UI_Base>(SubItemUI.UI_BattleVictory).gameObject.SetActive(true);
                });
                break;
            case Define.BattleResultType.Defeat:
                break;
            case Define.BattleResultType.Flee:
                break;
        }
    }

    public void OnTurnStart()
    {
        BattleActionPanel.InitTurn();
    }

    public void OnTurnEnd()
    {

    }
}
