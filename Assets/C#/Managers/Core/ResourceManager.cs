using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 파일 로드 및 GameObject 생성
public class ResourceManager
{
    // Resources폴더를 시작 위치로 path에 해당하는 에셋 파일을 로드하여 T 타입으로 반환
    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);

            GameObject go = Managers.PoolMng.GetOriginal(name);
            if (go != null)
                return go as T;
        }
        
        return Resources.Load<T>(path);
    }

    // Prefabs폴더를 시작 위치로 path에 해당하는 GameObject를 생성 후 부모 오브젝트를 parent 로 설정하여 반환
    public GameObject Instantiate(string path, Transform parent = null, string name = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        if (original.GetComponent<PoolAble>() != null)
            return Managers.PoolMng.Pop(original, parent).gameObject;

        GameObject go = Object.Instantiate(original, parent);

        if (name == null)
            go.name = original.name;
        else
            go.name = name;
        
        return go;
    }

    // go가 poolAble일 경우 Pool에 Push한 후 제거 
    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        PoolAble poolAble = go.GetComponent<PoolAble>();
        if (poolAble != null)
        {
            Managers.PoolMng.Push(poolAble);
            return;
        }
        
        Object.Destroy(go);
    }
}

/* 사용예제
    void Start()
    {
        GameObject mTemp = Managers.Resource.Instantiate("Temp");
        Managers.Resource.Destroy(mTemp);
    }
*/
