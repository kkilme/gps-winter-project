using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BattleScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        
        SceneType = Define.SceneType.BattleScene;

        TestSpawnHeroes();
        
        Managers.UIMng.ShowSceneUI<UI_BattleScene>();
        Managers.BattleMng.InitBattle(Define.MONSTERSQUAD_SQUAD1_ID);
        // TODO - TEST CODE
        Managers.ResourceMng.Instantiate("UI/SceneUI/UI_PlayerStatGroup");
    }

    public override void Clear()
    {
        Debug.Log("BattleScene Clear!");
    }

    // TODO - TEST CODE
    public void TestSpawnHeroes()
    {
        Hero hero1 = Managers.ObjectMng.SpawnHero(Define.HERO_KNIGHT_ID);
        Weapon weapon1 = new SampleSingleSword();
        weapon1.SetInfo(Define.WEAPON_SAMPLESINGLESWORD_ID);
        hero1.EquipWeapon(weapon1);
        
        // Hero hero2 = Managers.ObjectMng.SpawnHero(Define.HERO_KNIGHT_ID);
        // Weapon weapon2 = new SampleSingleSword();
        // weapon2.SetInfo(Define.WEAPON_SAMPLESINGLESWORD_ID);
        // hero2.EquipWeapon(weapon2);
        
        // Hero hero3 = Managers.ObjectMng.SpawnHero(Define.HERO_KNIGHT_ID);
        // Weapon weapon3 = new SampleSingleSword();
        // weapon3.SetInfo(Define.WEAPON_SAMPLESINGLESWORD_ID);
        // hero3.EquipWeapon(weapon3);
    }
}
