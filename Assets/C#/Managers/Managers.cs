using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static bool _initialized;
    
    private static Managers s_instance;
    public static Managers Instance { get { Init(); return s_instance; } }

    #region Contents
    private BattleManager _battleMng = new BattleManager();
    private AreaManager _areaMng = new AreaManager();
    private HeroManager _heroMng = new HeroManager();
    private TownManager _townMng = new TownManager();
    private InventoryManager _inventoryManager = new InventoryManager();

    public static BattleManager BattleMng => Instance._battleMng;
    public static AreaManager AreaMng => Instance._areaMng;
    public static TownManager TownMng => Instance._townMng;
    public static HeroManager HeroMng => Instance._heroMng;
    public static InventoryManager InvMng => Instance._inventoryManager;
    #endregion

    #region Core
    private DataManager _dataMng = new DataManager();
    private InputManager _inputMng = new InputManager();
    private ObjectManager _objectMng = new ObjectManager();
    private PoolManager _poolMng = new PoolManager();
    private ResourceManager _resourceMng = new ResourceManager();
    private SceneManagerEx _sceneMng = new SceneManagerEx();
    private SoundManager _soundMng = new SoundManager();
    private UIManager _uiMng = new UIManager();

    public static DataManager DataMng => Instance._dataMng;
    public static InputManager InputMng => Instance._inputMng;
    public static ObjectManager ObjectMng => Instance._objectMng;
    public static PoolManager PoolMng => Instance._poolMng;
    public static ResourceManager ResourceMng => Instance._resourceMng;
    public static SceneManagerEx SceneMng => Instance._sceneMng;
    public static SoundManager SoundMng => Instance._soundMng;
    public static UIManager UIMng => Instance._uiMng;
    
    #endregion

    public static void Init()
    {
        if (s_instance == null || !_initialized)
        {
            _initialized = true;
            
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
                go.AddComponent<CoroutineRunner>();
            }
            
            DontDestroyOnLoad(go);
            
            s_instance = go.GetComponent<Managers>();
        }
    }
    
    public static void Clear()
    {
        InputMng.Clear();
        SoundMng.Clear();
        UIMng.Clear();
        PoolMng.Clear();
    }
}
