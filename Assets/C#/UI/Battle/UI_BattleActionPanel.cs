using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 전투 중 액션 선택 창
/// </summary>
public class UI_BattleActionPanel : UI_Base
{
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

    private RectTransform _rect;
    private Transform _actionButtonParent;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));

        _actionButtonParent = GetGameObject(GameObjects.ActionButtons).transform;
        _rect = GetComponent<RectTransform>();
        _initialPosition = _rect.anchoredPosition; // 초기 위치 저장
    }

    private Tween _showTween = null;
    private Vector2 _initialPosition;

    public override Tween Show()
    {
        _hero = Managers.BattleMng.CurrentTurnCreature as Hero;
        if (_hero == null) return DOVirtual.DelayedCall(0, () => { });
        ClearActionInfo();
        SetupActionButtons();
        gameObject.SetActive(true);

        if (_showTween != null) return _showTween; // 이미 진행중이면 중복 실행 방지
        _showTween = _rect.DOAnchorPosY(-400f, 1f).From(true).SetEase(Ease.OutCirc).OnComplete(() => { _showTween = null; });

        return _showTween;
    }

    public override Tween Hide()
    {
        _rect.anchoredPosition = _initialPosition; // 초기 위치로 되돌리기
        _showTween = null;
        return base.Hide();
    }

    public override void ShowInstantly()
    {
        SetupActionButtons();
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 영웅(무기)이 가진 스킬 버튼들 생성 및 배치
    /// </summary>
    private void SetupActionButtons()
    {
        ClearActionButtons();
        foreach (BattleSkill skill in _hero.Weapon.Skills)
        {
            var actionButton = Managers.UIMng.MakeSubItemUI<UI_BattleActionButton>(_actionButtonParent, "Battle/" + nameof(UI_BattleActionButton));
            actionButton.SetSkill(skill);

            var image = actionButton.GetComponent<Image>();
            image.sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.ACTIONICON_PATH_PREFIX + skill.SkillData.IconPath);
        }
        ShowSkillInfo(_hero.Weapon.Skills[0]);
    }

    /// <summary>
    /// 마우스를 가져다 댄 스킬의 정보 표시
    /// </summary>
    public void ShowSkillInfo(BattleSkill skill)
    {
        ClearActionInfo();
        GetText(Texts.Text_ActionName).text = skill.SkillData.Name;
        GetText(Texts.Text_ActionDescription).text = skill.SkillData.Description;

        // Note: 스킬 정보 역시 DamageText나 ItemDetailUI처럼 따로 디자인용 클래스를 만드는 것도 괜찮아보임.
        if (skill.SkillData is AttackSkillData)
        {
            GetGameObject(GameObjects.Amount).SetActive(true);
            var skillData = skill.SkillData as AttackSkillData;
            GetText(Texts.Text_AmountWord).text = "Damage\nPer Slot";
            if (skillData.AttackType == AttackType.Physical)
            {
                GetText(Texts.Text_AmountNumber).color = GlobalValues.PHYSICAL_UI_ELEMENT_COLOR;
            }
            else if (skillData.AttackType == AttackType.Magic)
            {
                GetText(Texts.Text_AmountNumber).color = GlobalValues.MAGIC_UI_ELEMENT_COLOR;
            }
            GetText(Texts.Text_AmountNumber).text = skillData.DamagePerCoin.ToString();
        }
        else
        {
            GetGameObject(GameObjects.Amount).SetActive(false);
        }

        if (skill.SkillData.UsingStat != StatName.None)
        {
            GetGameObject(GameObjects.Percentage).SetActive(true);
            GetText(Texts.Text_SlotPercentageWord).text = "Percentage\nPer Slot";
            GetText(Texts.Text_SlotPercentage).text = _hero.CreatureStat.NameToStat(skill.SkillData.UsingStat).ToString();
        }

        Managers.BattleMng.UI.CoinTossDisplay.Show(skill.SkillData.CoinCount, skill.SkillData.UsingStat);
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
