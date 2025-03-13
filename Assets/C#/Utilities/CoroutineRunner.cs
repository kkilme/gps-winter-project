using System.Collections;
using UnityEngine;

// Non-Monobehaviour 클래스에서 Coroutine을 사용하기 위한 Runner 클래스
public class CoroutineRunner : MonoBehaviour
{
    public static CoroutineRunner Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public new Coroutine StartCoroutine(IEnumerator routine)
    {
        return base.StartCoroutine(routine);
    }
}
