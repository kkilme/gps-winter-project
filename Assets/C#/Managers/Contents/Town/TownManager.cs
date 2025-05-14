using System.Collections;
using UnityEngine;

public class TownManager
{
    public UI_TownScene UI { get; private set; }

    private Transform[] _heroPositions; // hero 스폰 위치

    public void Init()
    {
        GameObject[] heroSpawnPoints = GameObject.FindGameObjectsWithTag("TownHeroSpawnPoint");
        _heroPositions = new Transform[heroSpawnPoints.Length];
        for (int i = 0; i < heroSpawnPoints.Length; i++)
        {
            _heroPositions[heroSpawnPoints.Length - i - 1] = heroSpawnPoints[i].transform;
        }

        SpawnHeroes();

        UI = Managers.UIMng.ShowSceneUI<UI_TownScene>();
        UI.InitUIs();
    }

    public void SpawnHeroes()
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
