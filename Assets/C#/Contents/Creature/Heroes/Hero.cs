using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Creature
{
    public HeroStat HeroStat => CreatureStat as HeroStat;
    public int InstanceId => HeroStat.HeroInstanceId;
    public Weapon Weapon { get; protected set; }
    public Dictionary<ArmorType, Armor> Armors { get; protected set; }

    #region Outfit
    // 각종 장비 오브젝트의 부모 오브젝트들
    private GameObject _helmet;
    private GameObject _leftHand;
    private GameObject _rightHand;
    private GameObject _hair;

    private int _startBodyIndex; // 첫 영웅 Body 외형의 인덱스
    private int _startHairIndex; // 첫 영웅 헤어 외형의 인덱스

    private const int BODY_ARMOR_COUNT = 20; // 존재하는 모든 Body 외형 개수
    private const int HAIR_COUNT = 13; // 존재하는 모든 헤어 외형 개수
    private const int CLOAK_COUNT = 3; // 존재하는 모든 Cloak 외형 개수
    #endregion
    protected override void Init()
    {
        base.Init();

        _helmet = GlobalUtility.FindChild(gameObject, "Helmet", true);
        _leftHand = GlobalUtility.FindChild(gameObject, "weapon_l", true);
        _rightHand = GlobalUtility.FindChild(gameObject, "weapon_r", true);

        for (int i = 0; i < BODY_ARMOR_COUNT; i++)
        {
            if (gameObject.transform.GetChild(i).gameObject.activeSelf)
            {
                _startBodyIndex = i;
                break;
            }
        }

        _hair = GlobalUtility.FindChild(gameObject, "Hair", true);
        for (int i = 0; i < HAIR_COUNT; i++)
        {
            if (_hair.transform.GetChild(i).gameObject.activeSelf)
            {
                _startHairIndex = i;
                break;
            }
        }

        Armors = new Dictionary<ArmorType, Armor>();
        foreach (ArmorType type in (ArmorType[])Enum.GetValues(typeof(ArmorType)))
            Armors.TryAdd(type, null);
    }

    public override void SetData(int dataId)
    {
        CreatureData = Managers.DataMng.HeroDataDict[dataId];
    }

    public void SetData(int dataId, HeroStat savedStat)
    {
        SetData(dataId);

        CreatureStat = savedStat; // 저장된 스탯 적용

        gameObject.name = $"{InstanceId}_{CreatureData.Name}";
    }

    public override Tween LookFront(float duration = 0f)
    {
        return transform.DOLookAt(Managers.BattleMng.GridSystem.MonsterGrid[StandingCell.Row, 2 - StandingCell.Column].transform.position, duration).SetEase(Ease.OutQuad);
    }

    public override IEnumerator OnDead()
    {
        Animator.SetBool(GlobalValues.ANIMATION_PARAM_DEAD, true);
        if (Managers.SceneMng.CurrentScene is BattleScene)
        {
            Managers.BattleMng.RemoveHero(this, false);
        }
        yield return new WaitForSeconds(5f);

        gameObject.SetActive(false); // 파괴하지 않음
    }

    #region Weapon


    public void EquipWeapon(int weaponDataId)
    {
        Weapon weapon = new Weapon(weaponDataId);
        EquipWeapon(weapon);
    }

    public void EquipWeapon(Weapon equippingWeapon)
    {
        if (Weapon?.WeaponData.DataId == equippingWeapon.WeaponData.DataId) // 동일한 무기 장착 시 무시
            return;

        UnEquipWeapon();

        Weapon = equippingWeapon;
        ShowWeaponObject();
        ChangeAnimator();
    }

    public void UnEquipWeapon()
    {
        HideWeaponObject();

        if (Weapon == null || Weapon.DataId == GlobalValues.HERO_HANDWEAPON_ID) return;

        Weapon = null;
        EquipWeapon(GlobalValues.HERO_HANDWEAPON_ID); // 손 무기로 변경
    }

    /// <summary>
    /// 장착한 무기에 맞게 무기 외형 게임 오브젝트 활성화
    /// </summary>
    private void ShowWeaponObject()
    {
        int leftIndex = Weapon.WeaponData.LeftIndex;
        int rightIndex = Weapon.WeaponData.RightIndex;

        if (leftIndex != 0)
            _leftHand.transform.GetChild(leftIndex).gameObject.SetActive(true);

        if (rightIndex != 0)
            _rightHand.transform.GetChild(rightIndex).gameObject.SetActive(true);
    }

    private void HideWeaponObject()
    {
        for (int i = 0; i < _leftHand.transform.childCount; i++)
        {
            _leftHand.transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < _rightHand.transform.childCount; i++)
        {
            _rightHand.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Hero의 애니메이터 변경. 무기에 따라 애니메이터가 다름.
    /// </summary>
    private void ChangeAnimator()
    {
        string path = GlobalValues.HERO_ANIMATOR_PATH_PREFIX + Weapon.WeaponType;
        Animator.runtimeAnimatorController = Managers.ResourceMng.Load<RuntimeAnimatorController>(path);
        Animator.SetBool(GlobalValues.ANIMATION_PARAM_TOWNSCENE, Managers.SceneMng.CurrentScene is TownScene);
    }
    #endregion

    #region Armor

    public void EquipArmor(int armorDataId)
    {
        Armor armor = new Armor(armorDataId);
        EquipArmor(armor);
    }

    public void EquipArmor(Armor equippingArmor)
    {
        ArmorType armorType = equippingArmor.ArmorType;

        if (Armors[armorType]?.ArmorData.DataId == equippingArmor.ArmorData.DataId) // 동일한 장비 장착 시 무시
            return;

        UnEquipArmor(armorType);

        Armors[armorType] = equippingArmor;
        ShowArmorObject(armorType);
    }

    public void UnEquipArmor(ArmorType armorType)
    {
        if (Armors[armorType] == null)
            return;

        HideArmorObject(armorType);
        Armors[armorType] = null;
    }

    /// <summary>
    /// 장착한 장비에 맞게 장비 외형 게임 오브젝트 활성화
    /// </summary>
    private void ShowArmorObject(ArmorType armorType)
    {
        HideArmorObject(armorType);
        int idx = Armors[armorType].ArmorData.ArmorIndex;
        switch (armorType)
        {
            case ArmorType.Body:
                transform.GetChild(_startBodyIndex).gameObject.SetActive(false);
                transform.GetChild(idx).gameObject.SetActive(true);
                break;
            case ArmorType.Cloak:
                transform.GetChild(idx + 20).gameObject.SetActive(true);
                break;
            case ArmorType.Helmet:
                _hair.transform.GetChild(_startHairIndex).gameObject.SetActive(false);
                _helmet.transform.GetChild(idx).gameObject.SetActive(true);
                break;
        }
    }

    private void HideArmorObject(ArmorType armorType)
    {
        switch (armorType)
        {
            case ArmorType.Body:
                for (int i = 0; i < BODY_ARMOR_COUNT; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
                transform.GetChild(_startBodyIndex).gameObject.SetActive(true);
                break;
            case ArmorType.Cloak:
                for (int i = 0; i < CLOAK_COUNT; i++)
                {
                    transform.GetChild(i + BODY_ARMOR_COUNT).gameObject.SetActive(false);
                }
                break;
            case ArmorType.Helmet:
                for (int i = 0; i < _helmet.transform.childCount; i++)
                {
                    _helmet.transform.GetChild(i).gameObject.SetActive(false);
                }
                _hair.transform.GetChild(_startHairIndex).gameObject.SetActive(true);
                break;
        }
    }

    #endregion

    private void OnDestroy()
    {
        HeroStat.ClearBuffAndDebuffStats();
    }
}
