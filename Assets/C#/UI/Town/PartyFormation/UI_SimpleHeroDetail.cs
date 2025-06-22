using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;   

/// <summary>
/// PartyFormation UI에서 한 영웅의 정보를 보여주는 UI
/// </summary>
public class UI_SimpleHeroDetail : UI_Base, IPointerClickHandler
{
    enum Images
    {
        Image_Hero,
    }

    enum Texts
    {
        Text_HeroName,

        Text_OrderInParty,

        Text_BaseDamage,
        Text_HP,
        Text_PhysicalDefense,
        Text_MagicDefense,
        Text_Strength,
        Text_Vitality,
        Text_Dexterity,
        Text_Intelligence,
    }

    enum GameObjects
    {
        Indicator_OrderInParty,

        WeaponSlot,
        HelmetSlot,
        BodySlot,
        CloakSlot,
    }

    private UI_PartyFormation _partyFormationUI;
    private HeroInstanceData _bindingHeroData;

    public override void Init() {}

    public void LateInit(UI_PartyFormation partyFormationUI, HeroInstanceData heroData)
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        _partyFormationUI = partyFormationUI;
        _bindingHeroData = heroData;

        // 히어로 Stat 및 기본 정보 UI에 반영
        UpdateUI(heroData);

        // 장비 UI 초기화
        GetGameObject(GameObjects.WeaponSlot).GetOrAddComponent<UI_HeroEquipmentSlot>().LateInit(heroData, EquipmentType.Weapon, new DefaultItemSlotDesign(), false);
        GetGameObject(GameObjects.HelmetSlot).GetOrAddComponent<UI_HeroEquipmentSlot>().LateInit(heroData, EquipmentType.Helmet, new DefaultItemSlotDesign(), false);
        GetGameObject(GameObjects.BodySlot).GetOrAddComponent<UI_HeroEquipmentSlot>().LateInit(heroData, EquipmentType.Body, new DefaultItemSlotDesign(), false);
        GetGameObject(GameObjects.CloakSlot).GetOrAddComponent<UI_HeroEquipmentSlot>().LateInit(heroData, EquipmentType.Cloak, new DefaultItemSlotDesign(), false);

        heroData.Stat.OnStatChanged -= UpdateStatUI;
        heroData.Stat.OnStatChanged += UpdateStatUI;
    }

    public void UpdateUI(HeroInstanceData heroData)
    {
        GetText(Texts.Text_HeroName).text = heroData.CustomName;
        GetImage(Images.Image_Hero).sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.CREATURE_IMAGE_PATH_PREFIX + $"{heroData.ClassName}_Front");
        UpdateStatUI(heroData.Stat);
    }

    private void UpdateStatUI(CreatureStat stat)
    {
        GetText(Texts.Text_BaseDamage).text = stat.BaseDamage.ToString();
        GetText(Texts.Text_HP).text = stat.MaxHp.ToString();
        GetText(Texts.Text_PhysicalDefense).text = stat.PhysicalDefense.ToString();
        GetText(Texts.Text_MagicDefense).text = stat.MagicDefense.ToString();
        GetText(Texts.Text_Strength).text = stat.Strength.ToString();
        GetText(Texts.Text_Vitality).text = stat.Vitality.ToString();
        GetText(Texts.Text_Dexterity).text = stat.Dexterity.ToString();
        GetText(Texts.Text_Intelligence).text = stat.Intelligence.ToString();
    }

    public void SetOrderInParty(int order)
    {
        GetGameObject(GameObjects.Indicator_OrderInParty).SetActive(true);
        GetText(Texts.Text_OrderInParty).text = order.ToString();
    }

    public void DisableOrderInParty()
    {
        GetGameObject(GameObjects.Indicator_OrderInParty).SetActive(false);
    }

    private void OnDestroy()
    {
        if (_bindingHeroData != null) _bindingHeroData.Stat.OnStatChanged -= UpdateStatUI;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            _partyFormationUI.OnHeroDetailUIClicked(this);
        }
    }
}
