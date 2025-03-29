public abstract class Armor: Equipment
{
    public Data.ArmorData ArmorData => EquipmentData as Data.ArmorData;
    public ArmorType ArmorType { get; protected set; }
    public int ArmorIndex { get; protected set; }
    
    public override void SetData(int dataId)
    {
        EquipmentType = EquipmentType.Armor;
        EquipmentData = Managers.DataMng.ArmorDataDict[dataId];

        base.SetData(dataId);

        ArmorIndex = ArmorData.ArmorIndex;
    }
}
