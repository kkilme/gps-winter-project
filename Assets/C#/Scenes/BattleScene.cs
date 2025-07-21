using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BattleScene : BaseScene
{   
    protected override void Init()
    {
        SceneType = SceneType.BattleScene;
        base.Init();
    }

    public void InitBattle(int squadId, string battleFieldName)
    {
        Managers.BattleMng.Init(squadId, battleFieldName);
    }

#if UNITY_EDITOR
    private void Start()
    {
        if (Managers.SceneMng.LastSceneType == SceneType.UnknownScene)
        {
            Managers.SoundMng.PlayBGM("BattleBGM");
            int testSquadId = GlobalValues.MONSTERSQUAD_SQUAD1_ID;
            // TODO: for test
            // BattleScene에서 시작하여 플레이 테스트 시에만 실행
            for(int i = 0; i<7; i++) Managers.AreaMng.Items.Add(new HealPotion(GlobalValues.ITEM_HEALPOTION_ID)); // 테스트용 아이템 추가
            Managers.HeroMng.SpawnHeroParty();
            InitBattle(testSquadId + 2, Managers.DataMng.AreaDataDict[AreaName.Forest].BattleFieldName);
        }
    }

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
