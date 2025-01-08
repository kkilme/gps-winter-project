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
    }

    public override void Clear()
    {
        Debug.Log("BattleScene Clear!");
    }

    public void InitBattle(int squadId)
    {
        Managers.BattleMng.Init(squadId);
        Managers.UIMng.ShowSceneUI<UI_BattleScene>();
        Managers.UIMng.ShowPlayerProfileGroupUI(true);
    }

    //// TODO - TEST CODE
    //public void TestSpawnHeroes()
    //{
    //    Hero hero1 = Managers.ObjectMng.SpawnHero(Define.HERO_KNIGHT_ID);
    //    Weapon weapon1 = new SampleSingleSword();
    //    weapon1.SetInfo(Define.WEAPON_SAMPLESINGLESWORD_ID);
    //    hero1.EquipWeapon(weapon1);
    //}

    private void Start()
    {
        // TODO: BattleScene에서 시작하여 플레이 테스트 시에만 실행
        if (Managers.SceneMng.FirstScene != null && Managers.SceneMng.FirstScene is BattleScene)
        {   
            Managers.ObjectMng.SpawnHeroesOnTest();
            InitBattle(Define.MONSTERSQUAD_SQUAD1_ID);
        }
    }
}
