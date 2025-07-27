using System.Collections;
using UnityEngine;


public class AreaMapGenerateTestScene : BaseScene
{
    private AreaMapGenerator _areaMapGenerator;

    protected override void Init()
    {
        SceneType = SceneType.AreaScene;
        base.Init();

        _areaMapGenerator = GetComponent<AreaMapGenerator>();
    }

#if UNITY_EDITOR
    private void Start()
    {
        GenerateMap();
    }

    public new void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateMap();
        }
    }

    public void GenerateMap()
    {
        _areaMapGenerator.Init();
        _areaMapGenerator.GenerateMap();
    }
#endif
}
