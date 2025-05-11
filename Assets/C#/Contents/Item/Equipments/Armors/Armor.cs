public class Armor: Equipment
{
    public ArmorData ArmorData => EquipmentData as ArmorData;
    public ArmorType ArmorType { get; protected set; }

    public Armor(int dataId)
    {
        SetData(dataId);
    }

    public override void SetData(int dataId)
    {
        ItemType = ItemType.Armor;
        ItemData = Managers.DataMng.ArmorDataDict[dataId];
        
        base.SetData(dataId);
    }
}
