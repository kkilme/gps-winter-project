using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HeroParty 
{
    public List<Hero> Heroes { get; set; }
    public List<GameObject> HeroObjects { get; set; }
    public Dictionary<Hero, Vector2Int> BattlePositions { get; set; } // 전투 맵에서 배치되는 그리드 위치
    public int Gold { get; set; } // 골드는 파티가 공유

    private int[][] HERO_TILE_POS_OFFSETS =
    {
        new [] { 0, 1 },
        new [] { -1, 0 },
        new [] { 1, 0 },
        new [] { 0, -1 }
    };

    private int _nextHeroPosX = 0;
    private int _nextHeroPosY = 0;
    public HeroParty()
    {
        Heroes = new List<Hero>();
        HeroObjects = new List<GameObject>();
        BattlePositions = new Dictionary<Hero, Vector2Int>();
        Gold = 0;
    }

    public void AddHero(Hero hero)
    {
        Heroes.Add(hero);
        HeroObjects.Add(hero.gameObject);
        BattlePositions.Add(hero, new Vector2Int(_nextHeroPosX, _nextHeroPosY));
        if (_nextHeroPosX == 2)
        {
            _nextHeroPosX = 0;
            _nextHeroPosY++;
        }
        else
        {
            _nextHeroPosX++;
        }

    }

    // Area에서 시작 지점에 히어로 배치
    public void InitOnArea(Vector3 startPosition)
    {   
        for (int i = 0; i < HeroObjects.Count; i++)
        {
            HeroObjects[i].transform.LookAt(Vector3.forward);
            HeroObjects[i].transform.position = startPosition + new Vector3(HERO_TILE_POS_OFFSETS[i][0], 0, HERO_TILE_POS_OFFSETS[i][1]);
        }
    }

    public Sequence MoveTo(Vector3 destination)
    {
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < HeroObjects.Count; i++)
        {
            Vector3 adjustedDestination =
                destination + new Vector3(HERO_TILE_POS_OFFSETS[i][0], 0, HERO_TILE_POS_OFFSETS[i][1]);
            HeroObjects[i].transform.LookAt(adjustedDestination);
            sequence.Join(HeroObjects[i].transform.DOMove(adjustedDestination, 0.7f));
        }
        return sequence;
    }

    public void PlayMoveAnimation()
    {
        foreach (var hero in Heroes)
        {
            hero.PlayMoveAnimation();
        }
    }

    public void StopAnimation()
    {
        foreach (var hero in Heroes)
        {
            hero.StopAnimation();
        }
    }
}
