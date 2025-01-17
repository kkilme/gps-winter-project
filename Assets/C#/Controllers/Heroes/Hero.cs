using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Hero : Creature
{
    #region Field
    
    public Data.HeroData HeroData => CreatureData as Data.HeroData;
    public HeroStat HeroStat => (HeroStat)CreatureStat;

    public GameObject Head { get; protected set; }
    public GameObject LeftHand { get; protected set; }
    public GameObject RightHand { get; protected set; }

    public Bag Bag { get; protected set; }
    
    public Weapon Weapon { get; protected set; }
    public Define.WeaponType WeaponType => Weapon.WeaponType;
    public Dictionary<Define.ArmorType, Armor> Armors { get; protected set; }
    
    #endregion
    
    protected override void Init()
    {
        base.Init();
        
        Head = Util.FindChild(gameObject, "head", true);
        LeftHand = Util.FindChild(gameObject, "weapon_l", true);
        RightHand = Util.FindChild(gameObject, "weapon_r", true);

        Bag = new Bag();
        Bag.SetInfo();
        Bag.Owner = this;

        Armors = new Dictionary<Define.ArmorType, Armor>();
        foreach (Define.ArmorType type in (Define.ArmorType[])Enum.GetValues(typeof(Define.ArmorType)))
            Armors.TryAdd(type, null);
    }
    
    public override void SetInfo(int templateId)
    {
        CreatureType = Define.CreatureType.Hero;
        CreatureData = Managers.DataMng.HeroDataDict[templateId];
        base.SetInfo(templateId);
    }

    #region Battle
    
    public override void DoPrepareAction()
    {
        //((UI_BattleScene)Managers.UIMng.SceneUI).BattleActionPanel.InitTurn();
        
        //Managers.InputMng.MouseAction -= HandleMouseInput;
        //Managers.InputMng.MouseAction += HandleMouseInput;
    }
    
    public override void DoAction()
    {
        CurrentAction.DoAction();
    }

    public override void DoEndTurn()
    {
        //Managers.InputMng.MouseAction -= HandleMouseInput;
        
        //((UI_BattleScene)Managers.UIMng.SceneUI).BattleActionPanel.EndTurn();
        ((UI_BattleScene)Managers.UIMng.SceneUI).CoinTossUI.EndTurn();
        
        CreatureBattleState = Define.CreatureBattleState.Wait;
        CurrentAction.UnEquip();
        TargetCell = null;
        Managers.BattleMng.NextTurn();
    }

    #endregion

    // Prototype버전에선 Weapon, Armor 구현 X
    // TODO - Data Id로 무기 및 아머를 장착하도록 구현
    #region Weapon

    public void ChangeAnimator()
    {
        string path = "Animators/Players/" + WeaponType;
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
        HeroStat.AttachEquipment(Weapon.EquipmentData);
        Weapon.Equip(this);
        ChangeWeaponVisibility(true);
        ChangeAnimator();
    }
    
    public void UnEquipWeapon()
    {
        if (Weapon == null)
            return;
        
        HeroStat.DetachEquipment(Weapon.EquipmentData);
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
            LeftHand.transform.GetChild(leftIndex).gameObject.SetActive(isActive);
        }
        if (rightIndex != 0)
        {
            RightHand.transform.GetChild(rightIndex).gameObject.SetActive(isActive);
        }
    }

    #endregion

    
    #region Armor

    public void EquipArmor(Armor equippingArmor)
    {
        Define.ArmorType armorType = equippingArmor.ArmorType;
        if (Armors[armorType] != null)
        {
            if (Armors[armorType].ArmorData.DataId == equippingArmor.ArmorData.DataId)
                return;
            UnEquipArmor(armorType);
        }
        
        Armors[armorType] = equippingArmor;
        HeroStat.AttachEquipment(Armors[armorType].EquipmentData);
        Armors[armorType].Equip(this);
        ChangeArmorVisibility(armorType ,true);
    }

    public void UnEquipArmor(Define.ArmorType armorType)
    {
        if (Armors[armorType] == null)
            return;
        
        HeroStat.DetachEquipment(Armors[armorType].EquipmentData);
        Armors[armorType].UnEquip();
        ChangeArmorVisibility(armorType, false);
        Armors[armorType] = null;
    }

    public void ChangeArmorVisibility(Define.ArmorType armorType, bool isActive)
    {
        int idx = Armors[armorType].ArmorData.ArmorIndex;
        switch (armorType)
        {
             case Define.ArmorType.Body: 
                 transform.GetChild(idx - 1).gameObject.SetActive(isActive);
                 break; 
             case Define.ArmorType.Cloak:
                 transform.GetChild(idx + 19).gameObject.SetActive(isActive);
                 break;
             case Define.ArmorType.HeadAccessory:
                 Head.transform.GetChild(idx - 1).gameObject.SetActive(isActive);
                 break;
             case Define.ArmorType.Helmet:
                 Head.transform.GetChild(idx + 96).gameObject.SetActive(isActive);
                 break;
        }
    }
    
    #endregion
}
