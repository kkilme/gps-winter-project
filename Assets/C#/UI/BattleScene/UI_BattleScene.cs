using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System.Linq;

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
        ActionPanel = Get<UI_Base>(SubItemUI.UI_BattleActionPanel).GetComponent<UI_BattleActionPanel>();
        CoinTossDisplay = Get<UI_Base>(SubItemUI.UI_CoinTossDisplay).GetComponent<UI_CoinTossDisplay>();
        TurnstateUI = Get<UI_Base>(SubItemUI.UI_TurnState).GetComponent<UI_TurnState>();
        PlacementPhaseUI = Get<UI_Base>(SubItemUI.UI_PlacementPhase).GetComponent<UI_PlacementPhase>();
        ChooseTargetUI = Get<UI_Base>(SubItemUI.UI_ChooseTarget).GetComponent<UI_ChooseTarget>();
        HeroProfileGroupUI = Get<UI_Base>(SubItemUI.UI_HeroProfileGroup_Vertical).GetComponent<UI_HeroProfileGroup>();
        MonsterProfileGroupUI = Get<UI_Base>(SubItemUI.UI_MonsterProfileGroup).GetComponent<UI_MonsterProfileGroup>();
    }

    public void OnPlacementPhaseStart()
    {
        ActionPanel.Hide();
        CoinTossDisplay.Hide();
        ChooseTargetUI.Hide();
        TurnstateUI.Setup();
        TurnstateUI.HideInstantly();
        HeroProfileGroupUI.BindProfileUIs();
        MonsterProfileGroupUI.BindProfileUIs();
    }

    public void OnBattlePhaseStart()
    {
        TurnstateUI.Show();
        OnTurnStart();
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

    public void OnBattleEnd(BattleResultType battleResult)
    {
        ActionPanel.Hide();
        TurnstateUI.Hide();
        CoinTossDisplay.Hide();
        switch (battleResult)
        {
            case BattleResultType.Victory:
                Get<UI_Base>(SubItemUI.UI_BattleVictory).Show().OnComplete(() => { ShowReward(); });
                break;
            case BattleResultType.Defeat:
                break;
            case BattleResultType.Flee:
                break;
        }
    }

    private void ShowReward()
    {
        // TODO: Test code
        Dictionary<int, int> testrewards = new() { { 1, 2 }, { 2, 1 }, { 3, 1 } };
        UI_Reward rewardUI = Managers.UIMng.ShowPopupUI<UI_Reward>();

        KeyValuePair<int, int> reward = testrewards.Last();
        int rewardId = reward.Key;
        int rewardQuantity = reward.Value;

        rewardUI.Init(rewardId, rewardQuantity);


        testrewards.Remove(rewardId);

        void OnRewardAction(RewardActionType action)
        {
            Managers.UIMng.ClosePopupUI(rewardUI);
            switch (action)
            {
                case RewardActionType.Take:
                case RewardActionType.Dispose:
                    if (testrewards.Count == 0)
                    {
                        // TODO: BattleScene 언로딩 (AreaScene 복귀)
                    }
                    else
                    {
                        ShowReward();
                    }
                    break;
            }

        }

        rewardUI.OnRewardAction -= OnRewardAction;
        rewardUI.OnRewardAction += OnRewardAction;

    }


}
