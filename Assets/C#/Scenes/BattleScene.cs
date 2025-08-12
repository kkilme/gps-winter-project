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
    // Test용 코드 //////////////////////////////////////////////////

    [SerializeField] private int testSquadId = 0; // 테스트용 squadId
    private void Start()
    {
        if (Managers.SceneMng.LastSceneType == SceneType.UnknownScene)
        {
            Managers.SoundMng.PlayBGM("BattleBGM");
            for (int i = 0; i < 7; i++) Managers.AreaMng.Items.Add(new HealPotion(GlobalValues.ITEM_HEALPOTION_ID)); // 테스트용 아이템 추가
            Managers.HeroMng.SpawnHeroParty();
            if(Managers.DataMng.MonsterSquadDataDict.TryGetValue(testSquadId, out var squadData))
            {
                InitBattle(testSquadId, Managers.DataMng.AreaDataDict[AreaName.Forest].BattleFieldName);
            }
            else
            {
                InitBattle(GlobalValues.MONSTERSQUAD_SQUAD1_ID, Managers.DataMng.AreaDataDict[AreaName.Forest].BattleFieldName);
            }
        }
    }

    public new void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Managers.BattleMng.NextTurn());
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            Managers.BattleMng.FinishBattle(BattleResultType.Victory);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Managers.BattleMng.FinishBattle(BattleResultType.Defeat);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Managers.BattleMng.FinishBattle(BattleResultType.Retreat);
        }
    }
#endif
}
