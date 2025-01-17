using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;

public class UI_BattleActionPanel : UI_Base
{   
    enum GameObjects
    {
        ActionButtons
    }
    enum Texts
    {
        Text_ActionName,
        Text_ActionDescription,
        Text_AmountNumber,
        Text_AmountWord,
        Text_SlotPercentage,
        Text_SlotPercentageWord
    }

    public Action<BaseAction> OnMouseActionIconEntered { get; set; }
    private Hero _hero;
    private Transform _actionButtonParent;

    public override void Init()
    {   
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));

        _actionButtonParent = GetGameObject(GameObjects.ActionButtons).transform;
    }

    public void InitTurn()
    {
        gameObject.SetActive(true);
        _hero = Managers.BattleMng.CurrentTurnCreature as Hero;

        AddActionButtons();
    }

    public void EndTurn()
    {
        gameObject.SetActive(false);
        _hero = null;
    }

    protected void AddActionButtons()
    {   
        foreach (BaseAction action in _hero.Weapon.Actions)
        {
            var actionButton = Managers.ResourceMng.Instantiate("UI/SubItemUI/UI_ActionButton", _actionButtonParent).GetComponent<UI_ActionButton>();
            actionButton.Init(action);
            
            var image = actionButton.GetComponent<Image>();
            image.sprite = Managers.ResourceMng.Load<Sprite>($"Textures/Icons/{action.ActionData.IconName}");
        }
    }

    public void ShowActionInfo(BaseAction action)
    {
        ClearActionInfo();
        GetText(Texts.Text_ActionName).text = action.ActionData.Name;
        GetText(Texts.Text_ActionDescription).text = action.ActionData.Description;

        if (action.ActionData is Data.AttackActionData)
        {
            GetText(Texts.Text_AmountWord).text = "DAMAGE";
            GetText(Texts.Text_AmountNumber).text = _hero.HeroStat.Attack.ToString();
        }

        if (action.ActionData.UsingStat != Define.Stat.None)
        {
            GetText(Texts.Text_SlotPercentageWord).text = "Percentage\nPer Slot";
            GetText(Texts.Text_SlotPercentage).text = _hero.HeroStat.GetStatByDefine(action.UsingStat).ToString();
        }

        ((UI_BattleScene)Managers.UIMng.SceneUI).CoinTossUI.ShowCoinNum(action);
    }

    protected void ClearActionInfo()
    {
        GetText(Texts.Text_ActionName).text = null;
        GetText(Texts.Text_ActionDescription).text = null;
        GetText(Texts.Text_AmountWord).text = null;
        GetText(Texts.Text_AmountNumber).text = null;
        GetText(Texts.Text_SlotPercentageWord).text = null;
        GetText(Texts.Text_SlotPercentage).text = null;
    }
}
