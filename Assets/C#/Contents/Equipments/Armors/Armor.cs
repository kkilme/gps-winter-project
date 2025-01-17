public abstract class Armor: Equipment
{
    public Data.ArmorData ArmorData => EquipmentData as Data.ArmorData;
    public Define.ArmorType ArmorType { get; protected set; }
    public int ArmorIndex { get; protected set; }
    
    public override void SetInfo(int dataId)
    {
        EquipmentType = Define.EquipmentType.Armor;
        EquipmentData = Managers.DataMng.ArmorDataDict[dataId];

        base.SetInfo(dataId);

        ArmorIndex = ArmorData.ArmorIndex;
    }
}
