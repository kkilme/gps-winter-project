using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

// 전투 중 액션 선택 창
public class UI_BattleActionPanel : UI_Base
{
    private RectTransform _rect;
    enum GameObjects
    {
        ActionButtons,
        Amount,
        Percentage,
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
        SetupActionButtons();
        return _rect.DOAnchorPosY(-400f, 1f).From(true).SetEase(Ease.OutCirc).OnComplete(() => { });
    }

    public override void ShowInstantly()
    {
        SetupActionButtons();
        gameObject.SetActive(true);
    }

    // Hero(무기)가 가진 스킬 버튼들 생성 및 배치
    private void SetupActionButtons()
    {
        ClearActionButtons();
        foreach (BaseSkill skill in _hero.Weapon.Skills)
        {
            var actionButton = Managers.UIMng.MakeSubItemUI<UI_ActionButton>(_actionButtonParent, "Battle/" + nameof(UI_ActionButton));
            actionButton.SetSkill(skill);
            
            var image = actionButton.GetComponent<Image>();
            image.sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.ACTIONICON_PATH_PREFIX + skill.SkillData.IconPath);
        }
        ShowSkillInfo(_hero.Weapon.Skills[0]);
    }

    // 마우스를 가져다 댄 스킬의 정보 표시
    public void ShowSkillInfo(BaseSkill skill)
    {   
        ClearActionInfo();
        GetText(Texts.Text_ActionName).text = skill.SkillData.Name;
        GetText(Texts.Text_ActionDescription).text = skill.SkillData.Description;

        if (skill.SkillData is AttackSkillData)
        {
            GetGameObject(GameObjects.Amount).SetActive(true);
            var skillData = skill.SkillData as AttackSkillData;
            GetText(Texts.Text_AmountWord).text = "Damage\nPer Slot";
            if(skillData.AttackType == AttackType.Physical)
            {
                GetText(Texts.Text_AmountNumber).color = GlobalValues.PHYSICAL_UI_ELEMENT_BASE_COLOR;
            } else if (skillData.AttackType == AttackType.Magic)
            {
                GetText(Texts.Text_AmountNumber).color = GlobalValues.MAGIC_UI_ELEMENT_BASE_COLOR;
            }
            GetText(Texts.Text_AmountNumber).text = skillData.DamagePerCoin.ToString();
        } else
        {
            GetGameObject(GameObjects.Amount).SetActive(false);
        }

        if (skill.SkillData.UsingStat != StatName.None)
        {
            GetGameObject(GameObjects.Percentage).SetActive(true);
            GetText(Texts.Text_SlotPercentageWord).text = "Percentage\nPer Slot";
            GetText(Texts.Text_SlotPercentage).text = _hero.CreatureStat.NameToStat(skill.SkillData.UsingStat).ToString();
        }

        Managers.BattleMng.UI.CoinTossDisplay.ShowDeafult(skill.SkillData.CoinCount, skill.SkillData.UsingStat);
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
