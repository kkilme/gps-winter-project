using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UI_CreatureProfileGroup : UI_Base
{
    protected Dictionary<Creature, UI_CreatureProfile> _creatureProfiles = new Dictionary<Creature, UI_CreatureProfile>();

    public abstract void BindCreature();

    /// <summary>
    /// 모든 프로필 UI에 UI이벤트 바인딩
    /// </summary>
    public void BindEvent(Action<PointerEventData> action, UIEvent type = UIEvent.Click)
    {
        foreach(var profile in _creatureProfiles.Values)
        {            
            BindEvent(profile.gameObject, action, type);
        }
    }

    /// <summary>
    /// 모든 프로필 UI에서 이벤트 해제
    /// </summary>
    public void ClearEvent()
    {
        foreach (var profile in _creatureProfiles.Values)
        {
            ClearEvent(profile.gameObject);
        }
    }

    /// <summary>
    /// 모든 프로필 UI에 깜빡임 효과 재생
    /// </summary>
    public void StartBlinking()
    {
        foreach (var profile in _creatureProfiles.Values)
            profile.StartBlinking();
    }

    /// <summary>
    /// 특정 Creature의 프로필 UI에 깜빡임 효과 재생
    /// </summary>
    public void StartBlinking(Creature creature)
    {
        _creatureProfiles[creature].StartBlinking();
    }

    public void StopBlinking()
    {
        foreach (var profile in _creatureProfiles.Values)
            profile.StopBlinking();
    }

    public void OnDead(Creature creature)
    {
        _creatureProfiles[creature].OnDead();
    }

    public UI_CreatureProfile GetProfile(Creature creature)
    {
        if (_creatureProfiles.TryGetValue(creature, out var profile))
            return profile;
        
        Debug.LogError($"[UI_CreatureProfileGroup] Profile for {creature.name} not found.");
        return null;
    }
}
