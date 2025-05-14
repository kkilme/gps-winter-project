using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class SavedHeroData
{
    public int HeroDataId { get; set; } // HeroData의 DataId
    public int InstanceId { get; set; } // Hero의 InstanceId
    public string ClassName { get; set; } // Hero의 ClassName
    public HeroStat Stat { get; set; } // Hero의 Stat
    public int Weapon { get; set; } // 장착한 무기의 DataId // 무기나 장비가 강화나 내구도 등 '상태'를 저장해야 할 시 DataId가 아닌 다른 방식으로 저장할 필요가 있음.
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
