using System;
using UnityEngine;

public class TurnSystem
{
    public ulong[] Turns { get; protected set; }
    public int CurrentTurn { get; protected set; }
    public int CreatureCount { get; protected set; }

    public void Init(ulong[] turns, int count)
    {
        Turns = turns;
        CurrentTurn = 0;
        CreatureCount = count;
    }

    public void NextTurn()
    {
        CurrentTurn++;
        if (Turns[CurrentTurn] == 0)
            CurrentTurn = 0;
    }

    public Creature CurrentTurnCreature()
    {
        if (Managers.ObjectMng.Heroes.TryGetValue(Turns[CurrentTurn], out Hero hero))
            return hero;
        if (Managers.ObjectMng.Monsters.TryGetValue(Turns[CurrentTurn], out Monster monster))
            return monster;
        
        Debug.Log("Failed to get CurrentTurnCreature");
        return null;
    }
}
