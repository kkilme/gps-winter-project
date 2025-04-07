using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownScene : BaseScene
{
    [SerializeField] private Transform[] _heroPositions;
    protected override void Init()
    {
        base.Init();

        SceneType = SceneType.TownScene;
    }

    private void Start()
    {
        SpawnHeroes();

        Managers.UIMng.ShowSceneUI<UI_TownScene>();
    }

    private void SpawnHeroes()
    {
        Managers.HeroMng.AddHeroesOnTest();

        int i = 0;

        foreach (var hero in Managers.HeroMng.HeroParty.Heroes)
        {
            hero.gameObject.transform.position = _heroPositions[i++].position;
            hero.transform.LookAt(Camera.main.transform.position);
        }
    }

    public override void Clear()
    {
        #if UNITY_EDITOR
        Debug.Log("TownScene Clear!");
        #endif
    }
}
