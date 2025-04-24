using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GlobalUtility
{
    // go에 T컴포넌트가 있다면 가져오고, 없다면 생성하여 반환
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        
        return component;
    }

    // go의 모든 자식들 중 T컴포넌트를 가지며, name과 이름이 일치하는 GameObject 검색하여 반환
    // recursive가 true라면 바로 밑 자식 뿐 모든 자손들에서 검색, false라면 자식만 검색
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>(true))
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    // FindChild<Transform>로 실행하여 gameObject 검색 후 반환
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    // 2차원 정수 배열에서 최소값과 그 인덱스를 찾음. 최소값이 여러개일 시 랜덤으로 선택.
    public static void FindMinIndex(int[,] arr, out int x, out int y)
    {
        int minValue = arr.Cast<int>().Min();
        var minIndex =
            Enumerable.Range(0, arr.GetLength(0))
                .SelectMany(i => Enumerable.Range(0, arr.GetLength(1)).Select(j => (i, j)))
                .Where(t => arr[t.Item1, t.Item2] == minValue)
                .OrderBy(_ => Guid.NewGuid())
                .First();

        y = minIndex.Item1; x = minIndex.Item2;
    }

    public static void IncreaseDictCount<T>(Dictionary<T, int> dict, T key)
    {
        if (dict.ContainsKey(key))
        {
            dict[key]++;
        }
        else
        {
            dict.TryAdd(key, 1);
        }
    }

    public static Transform FindOrCreateTransform(string name)
    {
        GameObject transform = GameObject.Find(name);
        if (transform == null)
            transform = new GameObject { name = name };

        return transform.transform;
    }

    // UI의 하이어아키상에 다른 Canvas를 가지고 overrideSorting을 하는 오브젝트가 있는 경우 제대로 Sorting이 되지 않는 Unity 버그가 있음.
    // GameObject를 비활성화 후 다시 활성화하여 UI를 sorting을 강제로 갱신
    // WaitForEndOfFrame를 통해 렌더링이 끝난 후 UI를 갱신함
    public static IEnumerator FixUISorting(GameObject ui)
    {
        yield return new WaitForEndOfFrame();
        ui.SetActive(false);
        ui.SetActive(true);
    }
}
