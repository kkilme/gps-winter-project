using System;
using Unity.Netcode;
using UnityEngine;

public class MonsterStat: CreatureStat
{
    private int _speed;
    
    public int Speed { get => _speed; set { _speed = value; StatChangeAction?.Invoke(this); } }

    public override void SetStat(Data.CreatureData creatureData)
    {
        base.SetStat(creatureData);

        Data.MonsterData monsterData = (Data.MonsterData)creatureData;
        _speed = monsterData.Speed;
    }
} 