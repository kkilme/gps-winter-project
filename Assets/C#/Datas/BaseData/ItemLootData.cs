using System;


[Serializable]
public class ItemLootData // 아이템 또는 장비 전리품의 raw data
{
    public int ItemDataId;  // 아이템 또는 장비의 DataID
    public int DropChance; // 0~100 사이의 드랍 확률
}
