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

    private SavedHeroData _bindingHeroData;

    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
    }

    public void LateInit(SavedHeroData heroData)
    {
        _bindingHeroData = heroData;
        UpdateUI(heroData);

        GetGameObject(GameObjects.WeaponSlot).GetOrAddComponent<UI_HeroEquipmentSlot>().LateInit(heroData.InstanceId, EquipmentType.Weapon);
        GetGameObject(GameObjects.HelmetSlot).GetOrAddComponent<UI_HeroEquipmentSlot>().LateInit(heroData.InstanceId, EquipmentType.Helmet);
        GetGameObject(GameObjects.BodySlot).GetOrAddComponent<UI_HeroEquipmentSlot>().LateInit(heroData.InstanceId, EquipmentType.Body);
        GetGameObject(GameObjects.CloakSlot).GetOrAddComponent<UI_HeroEquipmentSlot>().LateInit(heroData.InstanceId, EquipmentType.Cloak);

        heroData.OnStatChanged -= UpdateUI;
        heroData.OnStatChanged += UpdateUI;
    }

    public void UpdateUI(SavedHeroData heroData)
    {
        GetImage(Images.Image_Hero).sprite = Managers.ResourceMng.Load<Sprite>(GlobalValues.CREATURE_IMAGE_PATH_PREFIX + $"{heroData.ClassName}_Front");
        GetText(Texts.Text_HeroName).text = heroData.CustomName;
        GetText(Texts.Text_BaseDamage).text = heroData.Stat.BaseDamage.ToString();
        GetText(Texts.Text_HP).text = heroData.Stat.MaxHp.ToString();
        GetText(Texts.Text_PhysicalDefense).text = heroData.Stat.PhysicalDefense.ToString();
        GetText(Texts.Text_MagicDefense).text = heroData.Stat.MagicDefense.ToString();
        GetText(Texts.Text_Strength).text = heroData.Stat.Strength.ToString();
        GetText(Texts.Text_Vitality).text = heroData.Stat.Vitality.ToString();
        GetText(Texts.Text_Dexterity).text = heroData.Stat.Dexterity.ToString();
        GetText(Texts.Text_Intelligence).text = heroData.Stat.Intelligence.ToString();
    }

    private void OnDestroy()
    {
        if(_bindingHeroData != null) _bindingHeroData.OnStatChanged -= UpdateUI;
    }

}
