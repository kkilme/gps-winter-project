using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;


public class HeroInstanceData
{
    public int HeroDataId { get; private set; } // HeroData의 DataId
    public int InstanceId { get; private set; } // Hero의 InstanceId
    public string ClassName => Managers.DataMng.HeroDataDict[HeroDataId].Name; // Hero의 ClassName: Prefab instantiate에 사용됨
    public string CustomName { get; set; } // 플레이어가 설정한 Hero의 이름
    public HeroStat Stat { get; set; } // Hero의 Stat
    public EquipmentInstanceData Weapon { get; set; } // 장착한 무기의 EquipmentInstanceData
    public Dictionary<ArmorType, EquipmentInstanceData> Armors { get; set; } // 장착한 ArmorType별 EquipmentInstanceData

    public Action<HeroInstanceData> OnEquipmentChanged { get; set; } // 장비가 변경될 때 호출되는 이벤트

    public HeroInstanceData(int heroDataId, int instanceId)
    {
        HeroDataId = heroDataId;
        InstanceId = instanceId;
        CustomName = ClassName;
        Stat = new HeroStat(Managers.DataMng.HeroDataDict[heroDataId], instanceId);

        // 시작 무기
        int startWeaponDataId = Managers.DataMng.HeroDataDict[heroDataId].StartWeapon;
        Managers.InvMng.AddItem(startWeaponDataId);
        Weapon = Managers.InvMng.GetUnequippedEquipment(startWeaponDataId);
        Weapon.EquippedHeroId = instanceId;

        Stat.AddEquipmentStat(Weapon.EquipmentData); 
        

        // 시작 방어구 (없음)
        Armors = new Dictionary<ArmorType, EquipmentInstanceData>
        {
            { ArmorType.Helmet, null},
            { ArmorType.Body, null},
            { ArmorType.Cloak, null}
        };
    }
}
