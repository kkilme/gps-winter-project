using System.Collections;
using UnityEngine;

// Non-Monobehaviour 클래스에서 Coroutine을 사용하기 위한 Runner 클래스
public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;
    public static CoroutineRunner Instance {
        get 
        { 
            if(_instance == null)
            {
                GameObject go = GameObject.Find("@CoroutineRunner");
                if (go == null)
                {
                    go = new GameObject { name = "@CoroutineRunner" };
                    DontDestroyOnLoad(go);
                }
                _instance = go.GetOrAddComponent<CoroutineRunner>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
    }

    public new Coroutine StartCoroutine(IEnumerator routine)
    {
        return base.StartCoroutine(routine);
    }
}
