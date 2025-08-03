public sealed class CollapsedTile : AreaEventTile
{
    public override void OnTileEnter()
    {
        if (_willBeDestroyed) return;

        ApplyCollapseDamage();

        CoroutineRunner.Instance.StartCoroutine(Managers.AreaMng.OnTileEventFinish());
    }

    public void ApplyCollapseDamage()
    {
        foreach (var hero in Managers.HeroMng.HeroParty.RuntimeHeroes)
            hero.TakeDamage(.15f, DamageTextType.NormalDamage);
    }
}
