public class Equipment
{
    public int DataId { get; protected set; }
    public Define.EquipmentType EquipmentType { get; protected set; }
    public Data.EquipmentData EquipmentData { get; protected set; }
    public Hero Owner { get; protected set; }
    
    // 수동 실행
    public virtual void SetInfo(int dataId)
    {
        DataId = dataId;
    }

    public void Equip(Hero hero)
    {
        Owner = hero;
    }

    public void UnEquip()
    {
        Owner = null;
    }
}
