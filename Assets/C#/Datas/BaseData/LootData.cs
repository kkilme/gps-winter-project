using System;
using System.Collections.Generic;

public enum LootType
{
    Gold,
    Item,
    Equipment
}

[Serializable]
public class ItemLootData // 아이템 또는 장비 전리품의 raw data
{
    public LootType Type; // Item 또는 Equipment
    public int DataId;    // 아이템 또는 장비의 DataID
    public int DropChance; // 0~100 사이의 드랍 확률
}

public class Loot // 결정된 전체 전리품
{
    public uint Gold;
    public List<Item> Items;

    public Loot()
    {
        Gold = 0;
        Items = new List<Item>();
    }

    public void Add(Loot loot)
    {
        Gold += loot.Gold;
        Items.AddRange(loot.Items);
    }
}
