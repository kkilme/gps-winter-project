public class Equipment: Item
{
    public EquipmentData EquipmentData => ItemData as EquipmentData;
    public Hero Owner { get; protected set; }
    
    public void Equip(Hero hero)
    {
        Owner = hero;
    }

    public void UnEquip()
    {
        Owner = null;
    }
}
