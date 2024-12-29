using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HeroParty 
{
    public List<Hero> Heroes { get; set; }
    public List<GameObject> HeroObjects { get; set; }
    public int Gold { get; set; } // 골드는 파티가 공유

    private int[][] _heroTilePositionOffset =
    {
        new [] { 0, 1 },
        new [] { -1, 0 },
        new [] { 1, 0 },
        new [] { 0, -1 }
    };

    public HeroParty()
    {
        Heroes = new List<Hero>();
        HeroObjects = new List<GameObject>();
        Gold = 0;
    }

    public void AddHero(Hero hero)
    {
        Heroes.Add(hero);
        HeroObjects.Add(hero.gameObject);
    }

    // Area에서 시작 지점에 히어로 배치
    public void InitOnArea(Vector3 startPosition)
    {   
        for (int i = 0; i < HeroObjects.Count; i++)
        {
            HeroObjects[i].transform.LookAt(Vector3.forward);
            HeroObjects[i].transform.position = startPosition + new Vector3(_heroTilePositionOffset[i][0], 0, _heroTilePositionOffset[i][1]);
        }
    }

    public Sequence MoveTo(Vector3 destination)
    {
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < HeroObjects.Count; i++)
        {
            Vector3 adjustedDestination =
                destination + new Vector3(_heroTilePositionOffset[i][0], 0, _heroTilePositionOffset[i][1]);
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
