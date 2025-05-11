/// <summary>
/// 전투 중 아이템 사용 액션
/// </summary>
public abstract class ItemAction : BaseAction
{
    public Item Item { get; protected set; }

    public void SetItem(Item item)
    {
        Item = item;
    }
}