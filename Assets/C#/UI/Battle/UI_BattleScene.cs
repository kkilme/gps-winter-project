using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_BattleScene : UI_Scene
{
	enum SubItemUI
	{
		UI_BattleActionPanel,
		UI_CoinTossDisplay,
		UI_TurnState,
        UI_PlacementPhase,
        UI_ChooseTarget,
        UI_HeroProfileGroup_Vertical,
        UI_MonsterProfileGroup,
        UI_BattleVictory,
        UI_BattleDefeat,
        UI_BattleRetreat
    }

    enum Images
    {
        FadeBG,
    }

	public UI_BattleActionPanel ActionPanel { get; protected set; }
	public UI_CoinTossDisplay CoinTossDisplay { get; protected set; }
    public UI_TurnState TurnstateUI { get; protected set; }
    public UI_PlacementPhase PlacementPhaseUI { get; protected set; }
    public UI_ChooseTarget ChooseTargetUI { get; protected set; }
    public UI_HeroProfileGroup HeroProfileGroupUI { get; protected set; }
    public UI_MonsterProfileGroup MonsterProfileGroupUI { get; protected set; }

    private BattleManager _battleManager;
    private Image _fadeBG;

    public override void Init()
    {
		Bind<UI_Base>(typeof(SubItemUI));
        Bind<Image>(typeof(Images));

        ActionPanel = Get<UI_Base>(SubItemUI.UI_BattleActionPanel).GetComponent<UI_BattleActionPanel>();
        CoinTossDisplay = Get<UI_Base>(SubItemUI.UI_CoinTossDisplay).GetComponent<UI_CoinTossDisplay>();
        TurnstateUI = Get<UI_Base>(SubItemUI.UI_TurnState).GetComponent<UI_TurnState>();
        PlacementPhaseUI = Get<UI_Base>(SubItemUI.UI_PlacementPhase).GetComponent<UI_PlacementPhase>();
        ChooseTargetUI = Get<UI_Base>(SubItemUI.UI_ChooseTarget).GetComponent<UI_ChooseTarget>();
        HeroProfileGroupUI = Get<UI_Base>(SubItemUI.UI_HeroProfileGroup_Vertical).GetComponent<UI_HeroProfileGroup>();
        MonsterProfileGroupUI = Get<UI_Base>(SubItemUI.UI_MonsterProfileGroup).GetComponent<UI_MonsterProfileGroup>();

        _battleManager = Managers.BattleMng;
        _fadeBG = GetImage(Images.FadeBG);
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
        var turnCreature = _battleManager.CurrentTurnCreature;

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
        _fadeBG.DOColor(new Color(_fadeBG.color.r, _fadeBG.color.g, _fadeBG.color.b, 0.9f), 0.7f).OnComplete(() =>
        {
            switch (battleResult)
            {
                case BattleResultType.Victory:
                    Get<UI_Base>(SubItemUI.UI_BattleVictory).Show().OnComplete(() => { ShowLoot(); });
                    break;
                case BattleResultType.Defeat:
                    Get<UI_Base>(SubItemUI.UI_BattleDefeat).Show();
                    break;
                case BattleResultType.Retreat:
                    Get<UI_Base>(SubItemUI.UI_BattleRetreat).Show();
                    break;
            }
        });
    }

    private void ShowLoot()
    {   
        Loot loot = Managers.BattleMng.GenerateLoot();
        UI_Loot lootUI = Managers.UIMng.ShowPopupUI<UI_Loot>();

        int itemCount = loot.Items.Count + 1;
        lootUI.Init(loot.Gold);
        lootUI.OnLootAction -= OnLootAction;
        lootUI.OnLootAction += OnLootAction;
        lootUI.Show();

        void OnLootAction(LootActionType action)
        {
            Managers.UIMng.ClosePopupUI(lootUI);
            itemCount--;

            if (itemCount == 0)
            {
                CoroutineRunner.Instance.StartCoroutine(Managers.SceneMng.EndBattleScene(BattleResultType.Victory));
            } 
            else
            {
                ShowNext();
            }
        }

        void ShowNext()
        {
            UI_Loot lootUI = Managers.UIMng.ShowPopupUI<UI_Loot>();
            lootUI.Init(loot.Items[itemCount - 1]);
            lootUI.OnLootAction -= OnLootAction;
            lootUI.OnLootAction += OnLootAction;
            lootUI.Show();
        }
    }
}
