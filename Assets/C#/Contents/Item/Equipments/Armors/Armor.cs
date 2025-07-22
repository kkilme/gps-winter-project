public class Armor: Equipment
{
    public ArmorData ArmorData => EquipmentData as ArmorData;
    public ArmorType ArmorType => ArmorData.ArmorType;

    public Armor(int dataId)
    {
        SetData(dataId);
    }
}
