public abstract class Armor: Equipment
{
    public Data.ArmorData ArmorData => EquipmentData as Data.ArmorData;
    public Define.ArmorType ArmorType { get; protected set; }
    public int ArmorIndex { get; protected set; }
    
    public override void SetInfo(int templateId)
    {
        EquipmentType = Define.EquipmentType.Armor;
        
        base.SetInfo(templateId);

        ArmorIndex = ArmorData.ArmorIndex;
    }
}
