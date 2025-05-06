using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownScene : BaseScene
{
    private Transform[] _heroPositions;
    protected override void Init()
    {
        base.Init();

        SceneType = SceneType.TownScene;
    }

    private void Start()
    {
        GameObject[] heroSpawnPoints = GameObject.FindGameObjectsWithTag("TownHeroSpawnPoint");
        _heroPositions = new Transform[heroSpawnPoints.Length];
        for (int i = 0; i < heroSpawnPoints.Length; i++)
        {
            _heroPositions[heroSpawnPoints.Length-i-1] = heroSpawnPoints[i].transform;
        }
        // TODO: 첫 게임 시작 시의 영웅 파티 구성 
        if(Managers.SceneMng.FirstScene is TownScene) Managers.HeroMng.AddHeroesOnTest();

        SpawnHeroes();

        Managers.UIMng.ShowSceneUI<UI_TownScene>();
    }

    private void SpawnHeroes()
    {
        int i = 0;
        Managers.HeroMng.SpawnHeroParty();
        foreach (var hero in Managers.HeroMng.HeroParty.Heroes)
        {   
            hero.gameObject.transform.position = _heroPositions[i++].position;
            hero.transform.LookAt(Camera.main.transform.position);
            hero.transform.rotation = Quaternion.Euler(0, hero.transform.rotation.eulerAngles.y, 0);
        }
    }
}
