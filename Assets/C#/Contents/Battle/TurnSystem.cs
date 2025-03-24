using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem
{
    public Creature CurrentTurnCreature { get; private set; }
    public List<Creature> Turns { get; protected set; }
    public int TurnCount { get; protected set; }
    private BattleManager _battleManager;

    public void Init()
    {
        _battleManager = Managers.BattleMng;
        TurnCount = 1;
        Turns = new List<Creature>(_battleManager.Creatures);
        Turns.Sort((a, b) => b.CreatureStat.Dexterity.CompareTo(a.CreatureStat.Dexterity)); // Dex 높은 순으로 턴 순서 정렬
        CurrentTurnCreature = Turns[0];
    }

    public void NextTurn()
    {
        TurnCount++;
        var current = CurrentTurnCreature;
        if(current == Turns[0]) // 현재 턴인 Creature가 이번 턴에 전투에서 이탈한 경우, 실행하지 않음
        {   
            Turns.RemoveAt(0);
            Turns.Add(current);
        }
        CurrentTurnCreature = Turns[0];
        CurrentTurnCreature.StandingCell.HighlightOutline(); // 현재 턴인 Creature의 Cell 강조표시
    }

    public void Remove(Creature creature)
    {
        Turns.Remove(creature);
    }
}
