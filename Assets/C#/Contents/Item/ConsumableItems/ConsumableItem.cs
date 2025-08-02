public class ConsumableItem : Item
{
    public ConsumableItemData ConsumableItemData => ItemData as ConsumableItemData;

    public ConsumableItem(int dataId)
    {
        SetData(dataId);
    }
}
