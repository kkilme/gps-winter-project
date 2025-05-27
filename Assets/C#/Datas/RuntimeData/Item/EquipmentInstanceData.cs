using System.Collections;
using UnityEngine;


public class EquipmentInstanceData : ItemInstanceData
{
    public EquipmentData EquipmentData => ItemData as EquipmentData;
    public EquipmentType EquipmentType => EquipmentData.EquipmentType;
    public int EquippedHeroId { get; set; } = -1; // 장착된 영웅의 InstanceID, -1이면 장착되지 않음
    public EquipmentInstanceData(int itemDataId, int instanceId): base(itemDataId, instanceId) {}
}
