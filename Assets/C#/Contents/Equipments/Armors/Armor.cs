public abstract class Armor: Equipment
{
    public Data.ArmorData ArmorData => EquipmentData as Data.ArmorData;
    public GlobalEnums.ArmorType ArmorType { get; protected set; }
    public int ArmorIndex { get; protected set; }
    
    public override void SetInfo(int dataId)
    {
        EquipmentType = GlobalEnums.EquipmentType.Armor;
        EquipmentData = Managers.DataMng.ArmorDataDict[dataId];

        base.SetInfo(dataId);

        ArmorIndex = ArmorData.ArmorIndex;
    }
}
