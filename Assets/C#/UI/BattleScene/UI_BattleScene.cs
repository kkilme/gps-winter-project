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
        UI_BattleVictory,
        UI_PlacementPhase,
        UI_ChooseTarget
    }

	public UI_BattleActionPanel BattleActionPanel { get; protected set; }
	public UI_CoinToss CoinTossUI { get; protected set; }
    public UI_TurnState TurnstateUI { get; protected set; }
    public UI_PlacementPhase PlacementPhaseUI { get; protected set; }
    public UI_ChooseTarget ChooseTargetUI { get; protected set; }

    public override void Init()
    {
        base.Init();

		Bind<UI_Base>(typeof(SubItemUI));
        Managers.UIMng.ShowPlayerProfileGroupUI(true);
        BattleActionPanel = Get<UI_Base>(SubItemUI.UI_BattleActionPanel).GetOrAddComponent<UI_BattleActionPanel>();
        CoinTossUI = Get<UI_Base>(SubItemUI.UI_CoinToss).GetOrAddComponent<UI_CoinToss>();
        TurnstateUI = Get<UI_Base>(SubItemUI.UI_TurnState).GetOrAddComponent<UI_TurnState>();
        PlacementPhaseUI = Get<UI_Base>(SubItemUI.UI_PlacementPhase).GetOrAddComponent<UI_PlacementPhase>();
        ChooseTargetUI = Get<UI_Base>(SubItemUI.UI_ChooseTarget).GetOrAddComponent<UI_ChooseTarget>();
    }

    public void OnPlacementPhaseStart()
    {
        BattleActionPanel.Hide();
        CoinTossUI.Hide();
        ChooseTargetUI.Hide();
        TurnstateUI.Setup();
    }

    public void OnBattlePhaseStart()
    {
        PlacementPhaseUI.Hide();
    }

    public void OnBattleEnd(BattleResultType battleResult)
    {
        switch (battleResult)
        {
            case BattleResultType.Victory:
                Get<UI_Base>(SubItemUI.UI_BattleActionPanel).gameObject.SetActive(false);
                Get<UI_Base>(SubItemUI.UI_CoinToss).gameObject.SetActive(false);

                // Turn 상태바 움직임을 통해 자연스럽게 숨기기
                // TODO: TurnStateUI로 기능 이동
                TurnstateUI.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 90), 1f).OnComplete(() =>
                {
                    TurnstateUI.gameObject.SetActive(false);
                    Get<UI_Base>(SubItemUI.UI_BattleVictory).gameObject.SetActive(true);
                });
                break;
            case BattleResultType.Defeat:
                break;
            case BattleResultType.Flee:
                break;
        }
    }

    public void OnTurnStart()
    {
        if(Managers.BattleMng.CurrentTurnCreature is Hero) BattleActionPanel.Show();
    }

    public void OnTurnEnd()
    {

    }
}
