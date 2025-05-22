using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class SavedHeroData
{
    public int HeroDataId { get; set; } // HeroData의 DataId
    public int InstanceId { get; set; } // Hero의 InstanceId
    public string ClassName => Managers.DataMng.HeroDataDict[HeroDataId].Name; // Hero의 ClassName: Prefab instantiate에 사용됨
    public string CustomName { get; set; } // 플레이어가 설정한 Hero의 이름
    public HeroStat Stat { get; set; } // Hero의 Stat
    public int Weapon { get; set; } // 장착한 무기의 DataId // 무기나 장비가 강화나 내구도 등 '상태'를 저장해야 할 시 DataId가 아닌 다른 방식으로 저장할 필요가 있음.
    public Dictionary<ArmorType, int> Armors { get; set; } // 장착한 ArmorType별 DataId

    public SavedHeroData(int heroDataId, int instanceId)
    {
        HeroDataId = heroDataId;
        InstanceId = instanceId;
        CustomName = ClassName;
        Stat = new HeroStat(Managers.DataMng.HeroDataDict[heroDataId], instanceId);
        Weapon = Managers.DataMng.HeroDataDict[heroDataId].StartWeapon;
        if (Managers.DataMng.WeaponDataDict.TryGetValue(Weapon, out WeaponData weaponData))
        {
            Stat.AddEquipmentStat(weaponData); 
        } else
        {
            Debug.LogWarning($"[SavedHeroData] Start Weapon Data not found. InstanceId: {InstanceId}, ClassName: {ClassName}, Weapon Data Id: {Weapon}");
        }
        Armors = new();
    }
}
