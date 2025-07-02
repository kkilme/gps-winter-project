using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
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
        UI_BattleRetreat,
        UI_BattleBag,
    }

    enum Images
    {
        FadeBG,
    }

	public UI_BattleActionPanel ActionPanel { get; protected set; }
	public UI_CoinTossDisplay CoinTossDisplay { get; protected set; }
    public UI_TurnState TurnStateUI { get; protected set; }
    public UI_PlacementPhase PlacementPhaseUI { get; protected set; }
    public UI_ChooseTarget_Battle ChooseTargetUI { get; protected set; }
    public UI_HeroProfileGroup HeroProfileGroupUI { get; protected set; }
    public UI_MonsterProfileGroup MonsterProfileGroupUI { get; protected set; }
    public UI_BattleBag BattleBagUI { get; protected set; }

    private BattleManager _battleManager => Managers.BattleMng;
    private Image _fadeBG;

    public override void Init()
    {
		Bind<GameObject>(typeof(SubItemUI));
        Bind<Image>(typeof(Images));

        ActionPanel = GetGameObject(SubItemUI.UI_BattleActionPanel).GetOrAddComponent<UI_BattleActionPanel>();
        CoinTossDisplay = GetGameObject(SubItemUI.UI_CoinTossDisplay).GetOrAddComponent<UI_CoinTossDisplay>();
        TurnStateUI = GetGameObject(SubItemUI.UI_TurnState).GetOrAddComponent<UI_TurnState>();
        PlacementPhaseUI = GetGameObject(SubItemUI.UI_PlacementPhase).GetOrAddComponent<UI_PlacementPhase>();
        ChooseTargetUI = GetGameObject(SubItemUI.UI_ChooseTarget).GetOrAddComponent<UI_ChooseTarget_Battle>();
        HeroProfileGroupUI = GetGameObject(SubItemUI.UI_HeroProfileGroup_Vertical).GetOrAddComponent<UI_HeroProfileGroup>();
        MonsterProfileGroupUI = GetGameObject(SubItemUI.UI_MonsterProfileGroup).GetOrAddComponent<UI_MonsterProfileGroup>();
        BattleBagUI = GetGameObject(SubItemUI.UI_BattleBag).GetOrAddComponent<UI_BattleBag>();
        GetGameObject(SubItemUI.UI_BattleVictory).GetOrAddComponent<UI_BattleVictory>();
        GetGameObject(SubItemUI.UI_BattleDefeat).GetOrAddComponent<UI_BattleDefeat>();
        GetGameObject(SubItemUI.UI_BattleRetreat).GetOrAddComponent<UI_BattleRetreat>();

        Bind<UI_Base>(typeof(SubItemUI));

        _fadeBG = GetImage(Images.FadeBG);
    }

    public void OnPlacementPhaseStart()
    {
        BattleBagUI.HideInstantly();
        ActionPanel.Hide();
        CoinTossDisplay.Hide();
        ChooseTargetUI.Hide();
        TurnStateUI.Setup();
        TurnStateUI.HideInstantly();
        HeroProfileGroupUI.BindCreature();
        MonsterProfileGroupUI.BindCreature();
    }

    public void OnBattlePhaseStart()
    {
        TurnStateUI.Show();
        OnTurnStart();
    }

    public void OnTurnStart()
    {
        var turnCreature = _battleManager.CurrentTurnCreature;

        TurnStateUI.RefreshTurnFramesPosition();

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

    /// <summary>
    /// 전투 종료 시 전투 결과에 따른 UI 표시.
    /// </summary>
    public void OnBattleEnd(BattleResultType battleResult)
    {
        ActionPanel.Hide();
        TurnStateUI.Hide();
        CoinTossDisplay.Hide();
        _fadeBG.DOColor(new Color(_fadeBG.color.r, _fadeBG.color.g, _fadeBG.color.b, 0.9f), 0.7f).OnComplete(() =>
        {
            switch (battleResult)
            {
                case BattleResultType.Victory:
                    Get<UI_Base>(SubItemUI.UI_BattleVictory).Show().OnComplete(() => { ShowLoot(); });
                    break;
                case BattleResultType.Defeat:
                    Get<GameObject>(SubItemUI.UI_BattleDefeat).GetComponent<UI_Base>().Show();
                    break;
                case BattleResultType.Retreat:
                    Get<GameObject>(SubItemUI.UI_BattleRetreat).GetComponent<UI_Base>().Show();
                    break;
            }
        });
    }

    /// <summary>
    /// 전투 승리 시 전리품 UI 표시.
    /// </summary>
    private void ShowLoot()
    {   
        Loot loot = Managers.BattleMng.GenerateLoot(); // 전리품 생성
        UI_Loot lootUI = Managers.UIMng.ShowPopupUI<UI_Loot>();

        lootUI.OnLootTakeComplete += OnLootTakeComplete;
        lootUI.Show(loot);

        static void OnLootTakeComplete(Loot lootTaken)
        {
            Managers.AreaMng.Loots.Add(lootTaken);
            Managers.UIMng.ClosePopupUI<UI_Loot>();
            Managers.BattleMng.UnloadBattleScene(BattleResultType.Victory);
        }
    }
}
