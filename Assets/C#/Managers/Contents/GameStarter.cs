using System.Collections;
using UnityEngine;

/// <summary>
/// 게임 시작 시 기본 영웅/아이템 등 지급
/// </summary>
/// 
// 또다른 테스트 환경을 구축한다면 GameStarter를 상속받는 클래스를 제작하면 될 듯.
public class GameStarter
{
    public void OnGameStart()
    {
        AddStartHeroes();
        AddStartItems();
        AddStartGold();
    }

    private void AddStartHeroes()
    {
        for (int i = 0; i < 2; i++)
        {
            Managers.HeroMng.HeroStorage.AddHero(GlobalValues.HERO_KNIGHT_ID);
        }

        for (int i = 0; i < 2; i++)
        {
            Managers.HeroMng.HeroStorage.AddHero(GlobalValues.HERO_WIZARD_ID);
        }

        foreach (var key in Managers.HeroMng.HeroStorage.GetAllOwnedHeroInstanceIds())
        {
            Managers.HeroMng.HeroParty.AddHero(key);
        }

        Managers.HeroMng.HeroStorage.AddHero(GlobalValues.HERO_KNIGHT_ID);
        Managers.HeroMng.HeroStorage.AddHero(GlobalValues.HERO_KNIGHT_ID);
        Managers.HeroMng.HeroStorage.AddHero(GlobalValues.HERO_WIZARD_ID);
        Managers.HeroMng.HeroStorage.AddHero(GlobalValues.HERO_WIZARD_ID);

    }

    private void AddStartItems()
    {
        Managers.InvMng.AddItem(GlobalValues.ITEM_HEALPOTION_ID, 13);
        Managers.InvMng.AddItem(GlobalValues.ARMOR_SAMPLEBODY1_ID, 2);
    }

    private void AddStartGold()
    {
        Managers.InvMng.AddGold(3000);
    }

}
