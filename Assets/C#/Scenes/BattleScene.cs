using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BattleScene : BaseScene
{   
    protected override void Init()
    {
        base.Init();
      
        SceneType = SceneType.BattleScene;
    }

    public override void Clear()
    {
        Debug.Log("BattleScene Clear!");
    }

    public void InitBattle(int squadId)
    {
        Managers.BattleMng.Init(squadId);
    }

    private void Start()
    {
        
        if (Managers.SceneMng.FirstScene != null && Managers.SceneMng.FirstScene is BattleScene)
        {
            // TODO: for test
            // BattleScene에서 시작하여 플레이 테스트 시에만 실행
            Managers.HeroMng.AddHeroesOnTest();
            Managers.HeroMng.SpawnHeroes();
            InitBattle(GlobalValues.MONSTERSQUAD_SQUAD1_ID);
        } else
        {
            InitBattle(GlobalValues.MONSTERSQUAD_SQUAD1_ID);
        }
    }

#if UNITY_EDITOR
    // TOOD: for test
    public new void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Managers.BattleMng.NextTurn());
        }
        if(Input.GetKeyDown(KeyCode.V))
        {
            Managers.BattleMng.FinishBattle(BattleResultType.Victory);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Managers.BattleMng.FinishBattle(BattleResultType.Defeat);
        }
    }
#endif
}
