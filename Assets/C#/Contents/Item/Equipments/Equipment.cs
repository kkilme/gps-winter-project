public class Equipment : Item
{
    public EquipmentData EquipmentData => ItemData as EquipmentData;
    public EquipmentType EquipmentType => EquipmentData.EquipmentType;
}
