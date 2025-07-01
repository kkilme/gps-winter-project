using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot // 결정된 전체 전리품
{
    public int Gold;
    public List<ItemData> Items; // 아이템의 DataId를 저장하는 리스트

    public Loot()
    {
        Gold = 0;
        Items = new List<ItemData>();
    }

    public void Add(Loot loot)
    {
        Gold += loot.Gold;
        Items.AddRange(loot.Items);
    }
}