using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 기본 Monster 클래스. 
/// </summary>
public class Monster : Creature
{
    public CreatureAI AIBrain { get; protected set; } // 자동 전투를 구현한다면 Creature로 옮겨야 할 듯
    public MonsterData MonsterData => CreatureData as MonsterData;
    
    public override void SetData(int dataId)
    {
        CreatureData = Managers.DataMng.MonsterDataDict[dataId];
        AIBrain = GetComponent<CreatureAI>();
        CreatureStat = new CreatureStat(CreatureData);
        gameObject.name = $"{CreatureData.DataId}_{CreatureData.Name}";
    }

    public override Tween LookFront(float duration = 0f)
    {
        return transform.DOLookAt(Managers.BattleMng.GridSystem.HeroGrid[StandingCell.Row, 2 - StandingCell.Column].transform.position, duration).SetEase(Ease.OutQuad);
    }

    public override IEnumerator OnDead()
    {
        Animator.SetBool(GlobalValues.ANIMATION_PARAM_DEAD, true);
        if (Managers.SceneMng.CurrentScene is BattleScene)
        {
            Managers.BattleMng.RemoveMonster(this);
        }
        yield return new WaitForSeconds(5f);

        gameObject.SetActive(false);
    }

    public Loot GetLoot()
    {
        Loot loot = new Loot();
        loot.Gold = Random.Range(MonsterData.MinGold, MonsterData.MaxGold + 1);
        foreach (ItemLootData lootData in MonsterData.LootTable)
        {
            if (Random.Range(0, 101) <= lootData.DropChance)
            {
                switch (lootData.Type)
                {
                    case LootType.Item: // TODO: 아이템 및 장비 객체 생성
                        //loot.Items.Add(Managers.ItemMng.GetItem(lootData.DataId));
                        break;
                    case LootType.Equipment:
                        //loot.Equipments.Add(Managers.ItemMng.GetEquipment(lootData.DataId));
                        break;
                }
            }
        }
        return loot;
    }
}
