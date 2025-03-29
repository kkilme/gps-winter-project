using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Monster : Creature
{
    public CreatureAI AIBrain { get; protected set; } // 자동 전투를 구현한다면 Creature로 옮겨야 할 듯
    public Data.MonsterData MonsterData => CreatureData as Data.MonsterData;
    
    public override void SetData(int dataId)
    {
        CreatureType = CreatureType.Monster;
        CreatureData = Managers.DataMng.MonsterDataDict[dataId];
        AIBrain = GetComponent<CreatureAI>();
        base.SetData(dataId);
    }

    public override Tween LookFront(float duration = 0f)
    {
        return transform.DOLookAt(Managers.BattleMng.GridSystem.HeroGrid[StandingCell.Row, 2 - StandingCell.Column].transform.position, duration).SetEase(Ease.OutQuad);
    }

    public Data.Loot GetLoot()
    {
        Data.Loot loot = new Data.Loot();
        loot.Gold = Random.Range(MonsterData.MinGold, MonsterData.MaxGold + 1);
        foreach (Data.ItemLootData lootData in MonsterData.LootTable)
        {
            if (Random.Range(0, 101) <= lootData.DropChance)
            {
                switch (lootData.Type)
                {
                    case Data.LootType.Item: // TODO: 아이템 및 장비 객체 생성
                        //loot.Items.Add(Managers.ItemMng.GetItem(lootData.DataId));
                        break;
                    case Data.LootType.Equipment:
                        //loot.Equipments.Add(Managers.ItemMng.GetEquipment(lootData.DataId));
                        break;
                }
            }
        }
        return loot;
    }
}
