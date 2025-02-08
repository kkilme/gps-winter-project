using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BattleScene : BaseScene
{   
    protected override void Init()
    {
        base.Init();
      
        SceneType = GlobalEnums.SceneType.BattleScene;
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
            Managers.ObjectMng.SpawnHeroesOnTest();
            InitBattle(GlobalValues.MONSTERSQUAD_SQUAD1_ID);
        } else
        {
            InitBattle(GlobalValues.MONSTERSQUAD_SQUAD1_ID);
        }
    }

    // TOOD: for test
    protected void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Managers.BattleMng.NextTurn();
        }
    }
}
