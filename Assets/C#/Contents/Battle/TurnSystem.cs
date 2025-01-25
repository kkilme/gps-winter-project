using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem
{
    public List<Creature> Turns { get; protected set; }
    public int CurrentTurn { get; protected set; }

    public void Init()
    {
        CurrentTurn = 0;
        var rawCreatures = Managers.BattleMng.Creatures;
        Turns = new List<Creature>(rawCreatures);
        Turns.Sort((a, b) => a.CreatureStat.Speed.CompareTo(b.CreatureStat.Speed));
    }

    public void NextTurn()
    {
        CurrentTurn++;
        var current = Turns[0];
        Turns.RemoveAt(0);
        Turns.Add(current);
    }
}
