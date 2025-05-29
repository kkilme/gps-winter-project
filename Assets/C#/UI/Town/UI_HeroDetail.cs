using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;   

public class UI_HeroDetail : UI_Base
{
    enum Images
    {
        Image_Hero,
    }

    enum Texts
    {
        Text_BaseDamage,
        Text_HP,
        Text_PhysicalDefense,
        Text_MagicDefense,
        Text_Strength,
        Text_Vitality,
        Text_Dexterity,
        Text_Intelligence,
    }
    enum TextInputs
    {
        Text_HeroName,
    }

    enum Buttons
    {
        Button_ChangeName,
    }

    enum GameObjects
    {
        Indicator_InParty,

        WeaponSlot,
        HelmetSlot,
        BodySlot,
        CloakSlot,
    }

    private HeroInstanceData _bindingHeroData;
    private TMP_InputField _heroNameInputField;


    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<TMP_InputField>(typeof(TextInputs));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
    }

    public void LateInit(HeroInstanceData heroData)
    {
        _bindingHeroData = heroData;

        // 이름 입력 필드 관련 초기화
        _heroNameInputField = Get<TMP_InputField>(TextInputs.Text_HeroName);
        _heroNameInputField.enabled = false;
        GetButton(Buttons.Button_ChangeName).onClick.AddListener(EnableNameEdit);
        _heroNameInputField.onEndEdit.AddListener(FinishNameEdit);

        // 히어로 Stat 및 기본 정보 UI에 반영
        UpdateUI(heroData);

        // 장비 UI 초기화
        GetGameObject(GameObjects.WeaponSlot).GetOrAddComponent<UI_HeroEquipmentSlot>().LateInit(heroData, EquipmentType.Weapon);
        GetGameObject(GameObjects.HelmetSlot).GetOrAddComponent<UI_HeroEquipmentSlot>().LateInit(heroData, EquipmentType.Helmet);
        GetGameObject(GameObjects.BodySlot).GetOrAddComponent<UI_HeroEquipmentSlot>().LateInit(heroData, EquipmentType.Body);
        GetGameObject(GameObjects.CloakSlot).GetOrAddComponent<UI_HeroEquipmentSlot>().LateInit(heroData, EquipmentType.Cloak);

        heroData.Stat.OnStatChanged -= UpdateStatUI;
        heroData.Stat.OnStatChanged += UpdateStatUI;
    }

    public void UpdateUI(HeroInstanceData heroData)
    {
        GetImage(Images.Image_Hero).sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.CREATURE_IMAGE_PATH_PREFIX + $"{heroData.ClassName}_Front");
        _heroNameInputField.text = heroData.CustomName;
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

    private void EnableNameEdit()
    {
        _heroNameInputField.enabled = true;
        _heroNameInputField.Select();
        GetButton(Buttons.Button_ChangeName).gameObject.SetActive(false);
    }

    private void FinishNameEdit(string changedName)
    {
        _heroNameInputField.enabled = false;
        GetButton(Buttons.Button_ChangeName).gameObject.SetActive(true);
        _bindingHeroData.CustomName = changedName.Trim();
    }

    private void OnDestroy()
    {
        if (_bindingHeroData != null) _bindingHeroData.Stat.OnStatChanged -= UpdateStatUI;
    }

}
