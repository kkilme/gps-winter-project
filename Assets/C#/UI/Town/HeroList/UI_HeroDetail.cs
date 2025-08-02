using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HeroList UI에서 한 영웅의 상세 정보를 보여주는 UI
/// </summary>
public class UI_HeroDetail : UI_Base
{
    enum Images
    {
        Image_Hero,
    }

    enum Texts
    {
        Text_HeroName,

        Text_BaseDamage,
        Text_HP,
        Text_PhysicalDefense,
        Text_MagicDefense,
        Text_Strength,
        Text_Vitality,
        Text_Dexterity,
        Text_Intelligence,
    }
    enum InputField
    {
        InputField_HeroName,
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

    public override void Init() { }

    public void BindHero(HeroInstanceData heroData)
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<TMP_InputField>(typeof(InputField));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        _bindingHeroData = heroData;

        // 영웅이 파티에 속해 있는지 여부에 따라 Indicator UI 표시
        GetGameObject(GameObjects.Indicator_InParty).SetActive(Managers.HeroMng.IsHeroInParty(heroData));

        // 이름 입력 필드 관련 초기화
        GetText(Texts.Text_HeroName).text = heroData.CustomName;
        _heroNameInputField = Get<TMP_InputField>(InputField.InputField_HeroName);
        _heroNameInputField.text = "";
        _heroNameInputField.gameObject.SetActive(false);
        GetButton(Buttons.Button_ChangeName).onClick.AddListener(EnableNameEdit);
        _heroNameInputField.onEndEdit.AddListener(FinishNameEdit);

        // 영웅 Stat 및 기본 정보 UI에 반영
        UpdateUI(heroData);

        // 장비 UI 초기화
        GetGameObject(GameObjects.WeaponSlot).GetOrAddComponent<UI_HeroEquipmentSlot>().LateInit(heroData, EquipmentType.Weapon, new PlusIconItemSlotDesign());
        GetGameObject(GameObjects.HelmetSlot).GetOrAddComponent<UI_HeroEquipmentSlot>().LateInit(heroData, EquipmentType.Helmet, new PlusIconItemSlotDesign());
        GetGameObject(GameObjects.BodySlot).GetOrAddComponent<UI_HeroEquipmentSlot>().LateInit(heroData, EquipmentType.Body, new PlusIconItemSlotDesign());
        GetGameObject(GameObjects.CloakSlot).GetOrAddComponent<UI_HeroEquipmentSlot>().LateInit(heroData, EquipmentType.Cloak, new PlusIconItemSlotDesign());

        // 영웅 Stat 변화에 따라 UI 업데이트
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
        GetText(Texts.Text_HeroName).gameObject.SetActive(false);
        GetButton(Buttons.Button_ChangeName).gameObject.SetActive(false);
        _heroNameInputField.gameObject.SetActive(true);
        _heroNameInputField.text = _bindingHeroData.CustomName;
        _heroNameInputField.Select();
    }

    private void FinishNameEdit(string changedName)
    {
        if (changedName != "") _bindingHeroData.CustomName = changedName.Trim(); // 이름 변경

        _heroNameInputField.gameObject.SetActive(false);
        GetButton(Buttons.Button_ChangeName).gameObject.SetActive(true);
        GetText(Texts.Text_HeroName).text = _bindingHeroData.CustomName;
        GetText(Texts.Text_HeroName).gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        if (_bindingHeroData != null) _bindingHeroData.Stat.OnStatChanged -= UpdateStatUI;
    }

}
