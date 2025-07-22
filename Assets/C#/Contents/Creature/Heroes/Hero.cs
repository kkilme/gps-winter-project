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

    // 각종 장비 오브젝트의 부모 오브젝트들
    private GameObject _head;
    private GameObject _leftHand;
    private GameObject _rightHand;
    
    protected override void Init()
    {
        base.Init();
        
        _head = GlobalUtility.FindChild(gameObject, "head", true);
        _leftHand = GlobalUtility.FindChild(gameObject, "weapon_l", true);
        _rightHand = GlobalUtility.FindChild(gameObject, "weapon_r", true);

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
        if (Weapon == null || Weapon.DataId == GlobalValues.HERO_HANDWEAPON_ID) return;

        HideWeaponObject();
        Weapon = null;
        EquipWeapon(GlobalValues.HERO_HANDWEAPON_ID); // 손 무기로 변경
    }
    
    /// <summary>
    /// 장착한 무기에 맞게 무기 게임 오브젝트를 활성화/비활성화
    /// </summary>
    public void ShowWeaponObject()
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
        ChangeArmorObjectVisibility(armorType ,true);
    }

    public void UnEquipArmor(ArmorType armorType)
    {
        if (Armors[armorType] == null)
            return;

        ChangeArmorObjectVisibility(armorType, false);
        Armors[armorType] = null;
    }

    public void ChangeArmorObjectVisibility(ArmorType armorType, bool isActive)
    {
        int idx = Armors[armorType].ArmorData.ArmorIndex;
        switch (armorType)
        {
             case ArmorType.Body: 
                 transform.GetChild(idx - 1).gameObject.SetActive(isActive);
                 break; 
             case ArmorType.Cloak:
                 transform.GetChild(idx + 19).gameObject.SetActive(isActive);
                 break;
             case ArmorType.Helmet:
                 _head.transform.GetChild(idx + 96).gameObject.SetActive(isActive);
                 break;
        }
    }

    #endregion

    private void OnDestroy()
    {
        HeroStat.ClearBuffAndDebuffStats();
    }
}
