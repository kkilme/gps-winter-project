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

    enum GameObjects
    {
        Indicator_InParty,

        WeaponSlot,
        HelmetSlot,
        BodySlot,
        CloakSlot,
    }

    private HeroInstanceData _bindingHeroData;

    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
    }

    public void LateInit(HeroInstanceData heroData)
    {
        _bindingHeroData = heroData;
        UpdateUI(heroData);

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
        GetText(Texts.Text_HeroName).text = heroData.CustomName;
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

    private void OnDestroy()
    {
        if(_bindingHeroData != null) _bindingHeroData.Stat.OnStatChanged -= UpdateStatUI;
    }

}
