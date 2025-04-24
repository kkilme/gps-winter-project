using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_CreatureProfileGroup : UI_Base
{
    protected Dictionary<Creature, UI_CreatureProfile> _creatureProfiles = new Dictionary<Creature, UI_CreatureProfile>();

    public abstract void BindHero();

    public void StopBlinking()
    {
        foreach (var profile in _creatureProfiles.Values)
            profile.StopBlinking();
    }

    public void StartBlinking(Creature creature)
    {
        _creatureProfiles[creature].StartBlinking();
    }

    public void OnDead(Creature creature)
    {
        _creatureProfiles[creature].OnDead();
    }
}
