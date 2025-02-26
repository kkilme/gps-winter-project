using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class UI_BattleActionPanel : UI_Base
{
    private RectTransform _rect;
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

    private Hero _hero;
    private Transform _actionButtonParent;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));

        _actionButtonParent = GetGameObject(GameObjects.ActionButtons).transform;
        _rect = GetComponent<RectTransform>();
    }

    public override Tween Show()
    {   
        gameObject.SetActive(true);
        _hero = Managers.BattleMng.CurrentTurnCreature as Hero;
        ClearActionInfo();

        return _rect.DOAnchorPosY(-400f, 1f).From(true).SetEase(Ease.OutCirc).OnComplete(() => { SetupActionButtons(); });
    }

    public override void ShowInstantly()
    {
        SetupActionButtons();
        gameObject.SetActive(true);
    }

    private void SetupActionButtons()
    {
        ClearActionButtons();
        foreach (BaseSkill skill in _hero.Weapon.Skills)
        {
            var actionButton = Managers.UIMng.MakeSubItemUI<UI_ActionButton>(_actionButtonParent, "Battle/" + nameof(UI_ActionButton));
            actionButton.SetSkill(skill);
            
            var image = actionButton.GetComponent<Image>();
            image.sprite = Managers.ResourceMng.Load<Sprite>($"Textures/Icons/{skill.SkillData.IconPath}");
        }
        ShowSkillInfo(_hero.Weapon.Skills[0]);
    }

    public void ShowSkillInfo(BaseSkill skill)
    {   
        ClearActionInfo();
        GetText(Texts.Text_ActionName).text = skill.SkillData.Name;
        GetText(Texts.Text_ActionDescription).text = skill.SkillData.Description;

        if (skill.SkillData is Data.AttackSkillData)
        {
            GetText(Texts.Text_AmountWord).text = "DAMAGE";
            GetText(Texts.Text_AmountNumber).text = _hero.CreatureStat.BaseDamage.ToString();
        }

        if (skill.SkillData.UsingStat != StatName.None)
        {
            GetText(Texts.Text_SlotPercentageWord).text = "Percentage\nPer Slot";
            GetText(Texts.Text_SlotPercentage).text = _hero.CreatureStat.NameToStat(skill.UsingStat).ToString();
        }

        ((UI_BattleScene)Managers.UIMng.SceneUI).CoinTossDisplay.Show(skill.SkillData.CoinCount, skill.SkillData.UsingStat);
    }

    protected void ClearActionInfo()
    {
        GetText(Texts.Text_ActionName).text = "";
        GetText(Texts.Text_ActionDescription).text = "";
        GetText(Texts.Text_AmountWord).text = "";
        GetText(Texts.Text_AmountNumber).text = "";
        GetText(Texts.Text_SlotPercentageWord).text = "";
        GetText(Texts.Text_SlotPercentage).text = "";
    }

    private void ClearActionButtons()
    {
        foreach (Transform child in _actionButtonParent)
            Destroy(child.gameObject);
    }
}
