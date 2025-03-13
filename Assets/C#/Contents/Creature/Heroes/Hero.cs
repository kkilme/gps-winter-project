using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Creature
{
    #region Field

    public Weapon Weapon { get; protected set; }
    public Dictionary<ArmorType, Armor> Armors { get; protected set; }
    private GameObject _head;
    private GameObject _leftHand;
    private GameObject _rightHand;
    
    #endregion
    
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
    
    public override void SetInfo(int dataId)
    {
        CreatureType = CreatureType.Hero;
        CreatureData = Managers.DataMng.HeroDataDict[dataId];
        base.SetInfo(dataId);
    }

    public override Tween LookOpponent(float duration = 0f)
    {
        return transform.DOLookAt(Managers.BattleMng.GridSystem.MonsterGrid[StandingCell.Row, 2 - StandingCell.Column].transform.position, duration);
    }

    // Prototype버전에선 많은 종류의 Weapon, Armor는 구현 X
    #region Weapon

    private void ChangeAnimator()
    {
        string path = $"{GlobalValues.HERO_ANIMATOR_PATH_ROOT}/{Weapon.WeaponType}";
        Animator.runtimeAnimatorController = Managers.ResourceMng.Load<RuntimeAnimatorController>(path);
    }

    public void EquipWeapon(int weaponDataId)
    {
        Weapon weapon = new Weapon();
        weapon.SetInfo(weaponDataId);
        EquipWeapon(weapon);
    }
    
    public void EquipWeapon(Weapon equippingWeapon)
    {
        if (Weapon != null)
        {
            if (Weapon.WeaponData.DataId == equippingWeapon.WeaponData.DataId)
                return;
            UnEquipWeapon();
        }
        
        Weapon = equippingWeapon;
        CreatureStat.AttachEquipment(Weapon.EquipmentData);
        Weapon.Equip(this);
        ChangeWeaponVisibility(true);
        ChangeAnimator();
    }
    
    public void UnEquipWeapon()
    {
        if (Weapon == null)
            return;
        
        CreatureStat.DetachEquipment(Weapon.EquipmentData);
        Weapon.UnEquip();
        ChangeWeaponVisibility(false);
        Weapon = null;
    }
    
    public void ChangeWeaponVisibility(bool isActive)
    {
        int leftIndex = Weapon.WeaponData.LeftIndex;
        int rightIndex = Weapon.WeaponData.RightIndex;
        if (leftIndex != 0)
        {
            _leftHand.transform.GetChild(leftIndex).gameObject.SetActive(isActive);
        }
        if (rightIndex != 0)
        {
            _rightHand.transform.GetChild(rightIndex).gameObject.SetActive(isActive);
        }
    }

    #endregion

    
    #region Armor

    public void EquipArmor(Armor equippingArmor)
    {
        ArmorType armorType = equippingArmor.ArmorType;
        if (Armors[armorType] != null)
        {
            if (Armors[armorType].ArmorData.DataId == equippingArmor.ArmorData.DataId)
                return;
            UnEquipArmor(armorType);
        }
        
        Armors[armorType] = equippingArmor;
        CreatureStat.AttachEquipment(Armors[armorType].EquipmentData);
        Armors[armorType].Equip(this);
        ChangeArmorVisibility(armorType ,true);
    }

    public void UnEquipArmor(ArmorType armorType)
    {
        if (Armors[armorType] == null)
            return;

        CreatureStat.DetachEquipment(Armors[armorType].EquipmentData);
        Armors[armorType].UnEquip();
        ChangeArmorVisibility(armorType, false);
        Armors[armorType] = null;
    }

    public void ChangeArmorVisibility(ArmorType armorType, bool isActive)
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
             case ArmorType.HeadAccessory:
                 _head.transform.GetChild(idx - 1).gameObject.SetActive(isActive);
                 break;
             case ArmorType.Helmet:
                 _head.transform.GetChild(idx + 96).gameObject.SetActive(isActive);
                 break;
        }
    }
    
    #endregion
}
