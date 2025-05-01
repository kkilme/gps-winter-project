using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// 4명의 영웅으로 구성된 파티 관리
public class HeroParty
{
    public List<int> HeroIds { get; private set; } = new(); // heroInstanceId 리스트
    public List<Hero> Heroes { get; private set; } = new(); // Area/Battle에서 사용되는 Hero 리스트 - 최대 Length 4
    public Dictionary<int, Hero> HeroesDict { get; private set; } = new(); // key: heroInstanceId, value: Hero - 최대 Length 4

    private Dictionary<int, Vector2Int> _battlePositions = new(); // key: heroInstanceId, value: Battle에서 배치되는 위치(col, row). 현재 파티에 포함되어 있지 않은 영웅의 위치 정보도 저장되어있음. 
    private Dictionary<int, Vector2Int> _battlePositionsCache = new(); // key: heroInstanceId, value: 좌표. 현재 파티에 포함되어 있는 영웅의 battlePosition만 저장되는 딕셔너리. - 최대 Length 4

    /// <summary>
    /// 파티에 영웅(Id) 추가. 최대 인원 초과 시 추가하지 못함.
    /// </summary>
    public void AddHero(int id)
    {
        if(HeroIds.Count >= GlobalValues.MAX_HERO_COUNT)
        {
            Debug.LogWarning($"[HeroParty] Cannot add more heroes. Max hero count is {GlobalValues.MAX_HERO_COUNT}");
            return;
        }
        HeroIds.Add(id);
    }

    /// <summary>
    /// 실제 게임에서 사용될 Hero 객체를 딕셔너리에 추가.
    /// </summary>
    public void AddRuntimeHero(int id, Hero hero)
    {
        if(HeroesDict.ContainsKey(id) || Heroes.Contains(hero))
        {
            Debug.LogWarning($"[HeroParty] Could not add RuntimeHero: {id} {hero.name}");
            return;
        }
        Heroes.Add(hero);
        HeroesDict.Add(id, hero);
    }

    /// <summary>
    /// 살아있는 영웅 리스트 반환.
    /// </summary>
    /// <returns></returns>
    public List<Hero> GetAliveHeroes()
    {
        return Heroes.Where(hero => !hero.IsDead()).ToList();
    }

    /// <summary>
    /// 영웅이 모두 죽었는지 확인.
    /// </summary>
    public bool IsAllDead()
    {
        bool flag = true;
        foreach(var hero in Heroes) flag &= hero.IsDead();

        return flag;
    }

    #region Battle
    /// <summary>
    /// 영웅의 BattleGrid 위치를 반환.
    /// </summary>
    public Vector2Int GetBattlePosition(int heroInstanceId)
    {
        // 캐시에 저장된 위치가 있을 경우 캐시된 위치 반환
        if (_battlePositionsCache.ContainsKey(heroInstanceId))
        {
            return _battlePositionsCache[heroInstanceId];
        }
        // 캐시에는 없지만 이미 저장된 위치가 있고, 그 위치가 이미 사용된 위치가 아닐 경우 저장된 위치 반환
        else if (_battlePositions.ContainsKey(heroInstanceId) && !_battlePositionsCache.Values.ToList().Contains(_battlePositions[heroInstanceId]))
        {
            _battlePositionsCache.Add(heroInstanceId, _battlePositions[heroInstanceId]);
            return _battlePositions[heroInstanceId];
        }
        // 캐시에도 없고, 저장된 위치도 없을 경우 빈 위치를 찾아 반환
        else
        {
            var usedPos = _battlePositionsCache.Values.ToList();
            for(int i = 0; i < GlobalValues.BATTLEGRID_ROW_COUNT; i++)
            {
                for (int j = 0; j < GlobalValues.BATTLEGRID_COL_COUNT; j++)
                {
                    Vector2Int pos = new Vector2Int(j, i);
                    if (!usedPos.Contains(pos)) // BattleGrid 칸의 수(6)는 파티 최대 인원 수(4)보다 크므로, 반드시 빈 위치가 존재함
                    {
                        _battlePositionsCache.Add(heroInstanceId, pos);
                        if(!_battlePositions.ContainsKey(heroInstanceId))
                            _battlePositions.Add(heroInstanceId, pos);
                        return pos;
                    }
                }
            }
        }

        Debug.LogError($"[HeroParty] No available battlegrid position for heroInstanceId: {heroInstanceId}");
        return new Vector2Int(-1, -1);
    }

    /// <summary>
    /// 영웅의 BattleGrid 위치를 저장. 이미 저장된 위치가 있을 경우 덮어씀.
    /// </summary>
    public void SaveBattlePosition(int heroInstanceId, Vector2Int position)
    {
        _battlePositions[heroInstanceId] = position;
        if(_battlePositionsCache.ContainsKey(heroInstanceId)) _battlePositionsCache[heroInstanceId] = position;
    }
    #endregion

    #region Area
    /// <summary>
    /// Area에서 특정 위치로 이동.
    /// </summary>
    public Tween MoveTo(Vector3 targetPos)
    {
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < Heroes.Count; i++)
        {
            Vector3 adjustedTargetPos =
                targetPos + new Vector3(GlobalValues.HERO_POS_ON_AREA_TILE_OFFSET[i, 0], 0, GlobalValues.HERO_POS_ON_AREA_TILE_OFFSET[i, 1]);
            Heroes[i].transform.LookAt(adjustedTargetPos);
            sequence.Join(Heroes[i].transform.DOMove(adjustedTargetPos, 0.7f));
        }
        PlayMovingAnimation();

        return sequence.Play();
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

    /// <summary>
    /// Area에서의 휴식
    /// </summary>
    public void Rest()
    {
        foreach (var hero in Heroes)
        {
            hero.TakeHeal(0.25f);
        }
    }
    #endregion
}
