public class TownScene : BaseScene
{
    protected override void Init()
    {
        SceneType = SceneType.TownScene;
        base.Init();
    }

    private void Start()
    {
        if (Managers.SceneMng.IsFirstScene)
        {
            Managers.SoundMng.PlayBGM("TownBGM");
            Managers.TownMng.Init();
        }
    }
}
