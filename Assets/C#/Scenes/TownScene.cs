using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownScene : BaseScene
{
    [SerializeField] private int[] _startHeroIds;
    [SerializeField] private Transform[] _heroPositions;
    protected override void Init()
    {
        base.Init();

        SceneType = Define.SceneType.TownScene;
    }

    private void Start()
    {
        SpawnHeroes();

        Managers.UIMng.ShowSceneUI<UI_TownScene>();
    }

    private void SpawnHeroes()
    {
        if (_startHeroIds.Length > 4)
        {
            Debug.LogError("Max number of heroes is 4!");
            return;
        }
        foreach (int id in _startHeroIds)
        {
             Managers.ObjectMng.SpawnHero(id);
        }

        int i = 0;

        foreach (var hero in Managers.ObjectMng.Heroes.Values)
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
