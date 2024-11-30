using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_BattleOrder : UI_Base
{
    enum Buttons
    {
        Skill1,
        Skill2,
        Skill3,
        Item,
        Move,
        Flee
    }

    enum Texts
    {
        Text_ActionName,
        Text_ActionDescription,
        Text_DamageNumber,
        Text_DamageWord,
        Text_SlotPercentage,
        Text_SlotPercentageWord
    }
    
    public Hero CurrentTurnHero { get; protected set; }
    
    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        
        GetButton(Buttons.Skill1).GetOrAddComponent<UI_ActionButton>().UIBattleOrder = this;
        GetButton(Buttons.Skill2).GetOrAddComponent<UI_ActionButton>().UIBattleOrder = this;
        GetButton(Buttons.Skill3).GetOrAddComponent<UI_ActionButton>().UIBattleOrder = this;
        GetButton(Buttons.Move).GetOrAddComponent<UI_ActionButton>().UIBattleOrder = this;
        GetButton(Buttons.Flee).GetOrAddComponent<UI_ActionButton>().UIBattleOrder = this;
    }

    public void InitTurn()
    {
        gameObject.SetActive(true);
        CurrentTurnHero = Managers.BattleMng.CurrentTurnCreature as Hero;
        
        SetButtons();
    }

    public void EndTurn()
    {
        gameObject.SetActive(false);
        CurrentTurnHero = null;
    }
    
    protected void SetButtons()
    {
        GetButton(Buttons.Skill1).GetOrAddComponent<UI_ActionButton>().Action = CurrentTurnHero.Weapon.Skill1;
        GetButton(Buttons.Skill2).GetOrAddComponent<UI_ActionButton>().Action = CurrentTurnHero.Weapon.Skill2;
        GetButton(Buttons.Skill3).GetOrAddComponent<UI_ActionButton>().Action = CurrentTurnHero.Weapon.Skill3;
        GetButton(Buttons.Move).GetOrAddComponent<UI_ActionButton>().Action =
            Managers.ObjectMng.Actions[Define.ACTION_MOVE_ID];
        GetButton(Buttons.Flee).GetOrAddComponent<UI_ActionButton>().Action = Managers.ObjectMng.Actions[Define.ACTION_FLEE_ID];
    }

    public void ShowActionInfo(BaseAction action)
    {
        ClearActionInfo();
        GetText(Texts.Text_ActionName).text = action.ActionData.Name;
        GetText(Texts.Text_ActionDescription).text = action.ActionData.Description;

        if (action.ActionData.IsAttack)
        {
            GetText(Texts.Text_DamageWord).text = "DAMAGE";
            GetText(Texts.Text_DamageNumber).text = CurrentTurnHero.HeroStat.Attack.ToString();
        }

        if (action.ActionData.UsingStat != Define.Stat.None)
        {
            GetText(Texts.Text_SlotPercentageWord).text = "Percentage\nPer Slot";
            GetText(Texts.Text_SlotPercentage).text = CurrentTurnHero.HeroStat.GetStatByDefine(action.UsingStat).ToString();
        }
        
        ((UI_BattleScene)Managers.UIMng.SceneUI).CoinTossUI.ShowCoinNum(action);
    }

    protected void ClearActionInfo()
    {
        GetText(Texts.Text_ActionName).text = null;
        GetText(Texts.Text_ActionDescription).text = null;
        GetText(Texts.Text_DamageWord).text = null;
        GetText(Texts.Text_DamageNumber).text = null;
        GetText(Texts.Text_SlotPercentageWord).text = null;
        GetText(Texts.Text_SlotPercentage).text = null;
    }
}
