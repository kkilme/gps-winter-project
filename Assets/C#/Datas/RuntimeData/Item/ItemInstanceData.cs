public class ItemInstanceData
{
    public int ItemDataId { get; protected set; } // ItemData의 DataId
    public int InstanceId { get; protected set; } // 아이템의 InstanceId
    public ItemData ItemData => Managers.DataMng.ItemDataDict[ItemDataId]; // 아이템 데이터
    public ItemType ItemType => ItemData.ItemType;
    public int Quantity { get; set; } = 1; // 아이템의 수량. 장비 인스턴스의 경우 1로 고정.

    public ItemInstanceData(int itemDataId, int instanceId)
    {
        ItemDataId = itemDataId;
        InstanceId = instanceId;
    }
}
