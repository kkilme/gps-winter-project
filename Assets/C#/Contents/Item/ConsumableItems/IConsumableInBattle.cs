/// <summary>
/// Battle에서 사용할 수 있는 ConsumableItem의 인터페이스
/// </summary>
public interface IConsumableInBattle
{
    public ItemAction ItemAction { get; protected set; }

    public void SetItemAction();
}