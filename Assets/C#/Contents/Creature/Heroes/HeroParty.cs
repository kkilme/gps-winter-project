using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class HeroParty
{
    public List<Hero> Heroes { get; set; }
    public Dictionary<Hero, Vector2Int> BattlePositions { get; set; } // 전투 맵에서 배치되는 그리드 위치
    public int Gold { get; set; } // 골드는 파티가 공유

    private int _nextHeroPosX = 0;
    private int _nextHeroPosY = 0;
    public HeroParty()
    {
        Heroes = new List<Hero>();
        BattlePositions = new Dictionary<Hero, Vector2Int>();
        Gold = 0;
    }

    public void AddHero(Hero hero)
    {
        Heroes.Add(hero);
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
        for (int i = 0; i < Heroes.Count; i++)
        {
            Heroes[i].transform.LookAt(Vector3.forward);
            Heroes[i].transform.position = startPosition + new Vector3(GlobalValues.HERO_TILE_POS_OFFSET[i, 0], 0, GlobalValues.HERO_TILE_POS_OFFSET[i, 1]);
        }
    }

    public Sequence MakeMoveToSequence(Vector3 destination)
    {
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < Heroes.Count; i++)
        {
            Vector3 adjustedDestination =
                destination + new Vector3(GlobalValues.HERO_TILE_POS_OFFSET[i, 0], 0, GlobalValues.HERO_TILE_POS_OFFSET[i, 1]);
            Heroes[i].transform.LookAt(adjustedDestination);
            sequence.Join(Heroes[i].transform.DOMove(adjustedDestination, 0.7f));
        }

        return sequence;
    }

    public void PlayMovingAnimation()
    {
        foreach (var hero in Heroes)
        {
            hero.Animator.SetBool(GlobalValues.ANIMATION_PARAM_MOVING, true);
        }
    }

    public void StopMovingAnimation()
    {
        foreach (var hero in Heroes)
        {
            hero.Animator.SetBool(GlobalValues.ANIMATION_PARAM_MOVING, false);
        }
    }
}
