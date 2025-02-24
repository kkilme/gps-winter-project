using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem
{
    public List<Creature> Turns { get; protected set; }
    public int TurnCount { get; protected set; }

    public void Init()
    {
        TurnCount = 1;
        Turns = new List<Creature>(Managers.BattleMng.Creatures);
        Turns.Sort((a, b) => a.CreatureStat.Dexterity.CompareTo(b.CreatureStat.Dexterity));
    }

    public void NextTurn()
    {
        TurnCount++;
        var current = Turns[0];
        Turns[0].StandingCell.RevertOutlineColor();
        Turns.RemoveAt(0);
        Turns.Add(current);
        Turns[0].StandingCell.HighlightOutline();
    }
}
