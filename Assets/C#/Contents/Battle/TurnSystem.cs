using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem
{
    public Creature CurrentTurnCreature => Turns[0];
    public List<Creature> Turns { get; protected set; }
    public int TurnCount { get; protected set; }
    private BattleManager _battleManager;

    public void Init()
    {
        _battleManager = Managers.BattleMng;
        TurnCount = 1;
        Turns = new List<Creature>(_battleManager.Creatures);
        Turns.Sort((a, b) => b.CreatureStat.Dexterity.CompareTo(a.CreatureStat.Dexterity));
    }

    public void NextTurn()
    {
        TurnCount++;
        var current = CurrentTurnCreature;
        CurrentTurnCreature.StandingCell.RevertOutlineColor();
        Turns.RemoveAt(0);
        Turns.Add(current);
        CurrentTurnCreature.StandingCell.HighlightOutline(); // 현재 턴인 Creature의 Cell 강조표시
    }
}
