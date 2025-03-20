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
		UI_CoinTossDisplay,
		UI_TurnState,
        UI_BattleVictory,
        UI_PlacementPhase,
        UI_ChooseTarget,
        UI_HeroProfileGroup_Vertical,
        UI_MonsterProfileGroup,
    }

	public UI_BattleActionPanel ActionPanel { get; protected set; }
	public UI_CoinTossDisplay CoinTossDisplay { get; protected set; }
    public UI_TurnState TurnstateUI { get; protected set; }
    public UI_PlacementPhase PlacementPhaseUI { get; protected set; }
    public UI_ChooseTarget ChooseTargetUI { get; protected set; }
    public UI_HeroProfileGroup HeroProfileGroupUI { get; protected set; }
    public UI_MonsterProfileGroup MonsterProfileGroupUI { get; protected set; }

    public override void Init()
    {
        base.Init();

		Bind<UI_Base>(typeof(SubItemUI));
        ActionPanel = Get<UI_Base>(SubItemUI.UI_BattleActionPanel).GetOrAddComponent<UI_BattleActionPanel>();
        CoinTossDisplay = Get<UI_Base>(SubItemUI.UI_CoinTossDisplay).GetOrAddComponent<UI_CoinTossDisplay>();
        TurnstateUI = Get<UI_Base>(SubItemUI.UI_TurnState).GetOrAddComponent<UI_TurnState>();
        PlacementPhaseUI = Get<UI_Base>(SubItemUI.UI_PlacementPhase).GetOrAddComponent<UI_PlacementPhase>();
        ChooseTargetUI = Get<UI_Base>(SubItemUI.UI_ChooseTarget).GetOrAddComponent<UI_ChooseTarget>();
        HeroProfileGroupUI = Get<UI_Base>(SubItemUI.UI_HeroProfileGroup_Vertical).GetOrAddComponent<UI_HeroProfileGroup>();
        MonsterProfileGroupUI = Get<UI_Base>(SubItemUI.UI_MonsterProfileGroup).GetOrAddComponent<UI_MonsterProfileGroup>();
    }

    public void OnPlacementPhaseStart()
    {
        ActionPanel.Hide();
        CoinTossDisplay.Hide();
        ChooseTargetUI.Hide();
        TurnstateUI.Setup();
        TurnstateUI.HideInstantly();
        HeroProfileGroupUI.BindHeroProfileUIs();
        MonsterProfileGroupUI.BindMonsterProfileUIs();
    }

    public void OnBattlePhaseStart()
    {
        TurnstateUI.Show();
        OnTurnStart();
    }

    public void OnBattleEnd(BattleResultType battleResult)
    {
        switch (battleResult)
        {
            case BattleResultType.Victory:
                Get<UI_Base>(SubItemUI.UI_BattleActionPanel).gameObject.SetActive(false);
                //Get<UI_Base>(SubItemUI.UI_CoinToss).gameObject.SetActive(false);

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
        var turnCreature = Managers.BattleMng.CurrentTurnCreature;

        TurnstateUI.RefreshTurnFramesPosition();

        if (turnCreature is Hero)
        {
            HeroProfileGroupUI.StartBlinking(turnCreature as Hero);
            ActionPanel.Show();
        }
        else
        {
            MonsterProfileGroupUI.StartBlinking(turnCreature as Monster);
            ActionPanel.Hide();
        }
    }

    public void OnTurnEnd()
    {
        HeroProfileGroupUI.StopBlinking();
        MonsterProfileGroupUI.StopBlinking();
    }
}
