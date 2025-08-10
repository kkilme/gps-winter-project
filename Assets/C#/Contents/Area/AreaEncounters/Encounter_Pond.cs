public class Encounter_Pond : AreaEncounter
{
    protected override void AddResultLines()
    {
        int statValue = Managers.HeroMng.HeroParty.GetAverageStat(_usingStat);

        _popup.AddResultDetailLine(
            4,
            CoinTossHelper.CalculateChanceOfCoinTossResultAsPercent(_coinCount, 4, statValue),
            "Restores 40% of all heroes' hp."
        );

        _popup.AddResultDetailLine(
            3,
            CoinTossHelper.CalculateChanceOfCoinTossResultAsPercent(_coinCount, 3, statValue),
            "Restores 20% of all heroes' hp."
        );

        _popup.AddResultDetailLine(
            2,
            CoinTossHelper.CalculateChanceOfCoinTossResultAsPercent(_coinCount, 2, statValue),
            "Nothing happens."
        );

        _popup.AddResultDetailLine(
            1,
            CoinTossHelper.CalculateChanceOfCoinTossResultAsPercent(_coinCount, 1, statValue),
            "All heroes get damaged by 20% of max Hp."
        );

        _popup.AddResultDetailLine(
            0,
            CoinTossHelper.CalculateChanceOfCoinTossResultAsPercent(_coinCount, 0, statValue),
            "All heroes get damaged by 40% of max Hp."
        );
    }

    protected override void ExecuteResult(int coinSuccessCount)
    {
        switch (coinSuccessCount)
        {
            case 4:
                foreach (Hero hero in Managers.HeroMng.HeroParty.RuntimeHeroes)
                {
                    hero.TakeHeal(.4f);
                }
                break;
            case 3:
                foreach (Hero hero in Managers.HeroMng.HeroParty.RuntimeHeroes)
                {
                    hero.TakeHeal(.2f);
                }
                break;
            case 2: break;
            case 1:
                foreach (Hero hero in Managers.HeroMng.HeroParty.RuntimeHeroes)
                {
                    hero.TakeDamage(.2f, DamageTextType.NormalDamage);
                }
                break;
            case 0:
                foreach (Hero hero in Managers.HeroMng.HeroParty.RuntimeHeroes)
                {
                    hero.TakeDamage(.4f, DamageTextType.NormalDamage);
                }
                break;
        }
    }
}
