using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class SavedHeroData
{
    public int HeroDataId { get; set; } // HeroData의 DataId
    public int InstanceId { get; set; } // Hero의 InstanceId
    public string ClassName { get; set; } // Hero의 ClassName
    public HeroStat Stat { get; set; } // Hero의 Stat
    public int Weapon { get; set; } // 장착한 무기의 DataId
    public Dictionary<ArmorType, int> Armors { get; set; } // 장착한 ArmorType별 DataId

    public SavedHeroData(int heroDataId, int instanceId)
    {
        HeroDataId = heroDataId;
        InstanceId = instanceId;
        ClassName = Managers.DataMng.HeroDataDict[heroDataId].Name;
        Stat = new HeroStat(Managers.DataMng.HeroDataDict[heroDataId], instanceId);
        Weapon = Managers.DataMng.HeroDataDict[heroDataId].StartWeapon;
        Armors = new();
    }
}
