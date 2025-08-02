using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

/// <summary>
/// 모든 Scene의 조상 클래스. 각 Scene에 진입했을 때 가장 먼저 BaseScene이 Awake()가 호출된다.
/// </summary>
public abstract class BaseScene : MonoBehaviour
{
    public SceneType SceneType { get; protected set; } = SceneType.UnknownScene;

    private void Awake()
    {
        Init();
    }

    protected void Update()
    {
        Managers.InputMng.OnUpdate();
    }

    protected virtual void Init()
    {
        // TODO- 최초 Scene에서만 실행
        if (!Managers.ObjectHolder.Initialized)
        {
            Managers.Init();
            Managers.InputMng.Init();
            Managers.SceneMng.Init();
            Managers.DataMng.Init();
            Managers.SoundMng.Init();
            Managers.PoolMng.Init();
            Managers.UIMng.Init();
            Managers.ObjectHolder.Init();
            Managers.ResourceMng.Init();

            GameStarter game = new();
            game.SetupGame();
        }

        Object obj = FindObjectOfType(typeof(EventSystem));

        if (obj == null)
            Managers.ResourceMng.Instantiate("UI/EventSystem").name = "@EventSystem";
    }
}