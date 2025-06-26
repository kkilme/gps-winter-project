/// <summary>
/// Battle에서 사용할 수 있는 Item의 인터페이스
/// </summary>
public interface IUsableInBattle
{
    /// <summary>
    /// Battle에서 사용되는 아이템 액션을 반환.
    /// </summary>
    public ItemAction GetItemAction();
}