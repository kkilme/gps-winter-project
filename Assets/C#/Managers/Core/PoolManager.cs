using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ResourceManger를 보조하여 Pool 객체들을 관리
public class PoolManager
{
    private Dictionary<string, Pool> _poolDic = new Dictionary<string, Pool>();
    private Transform _root;

    public void Init()
    {
        if (_root == null)
        {
            _root = new GameObject { name = "@Pool_Root" }.transform;
            Object.DontDestroyOnLoad(_root);
        }
    }

    // original의 Pool을 count만큼 생성
    public void CreatePool(GameObject original, int count = 5)
    {
        Pool pool = new Pool();
        pool.Init(original, count);
        pool.Root.parent = _root;
        
        _poolDic.Add(original.name, pool);
    }

    // 다 사용한 poolable오브젝트를 Pool에 다시 넣어 대기 상태로 전환
    public void Push(PoolAble poolAble)
    {
        string name = poolAble.gameObject.name;
        if (_poolDic.ContainsKey(name) == false)
        {
            GameObject.Destroy(poolAble.gameObject);
            return;
        }
        
        _poolDic[name].Push(poolAble);
    }

    // original의 이름에 해당하는 Pool을 parent를 부모 오브젝트로 설정한 후 Pop
    public PoolAble Pop(GameObject original, Transform parent = null)
    {
        if(_poolDic.ContainsKey(original.name) == false)
            CreatePool(original);

        return _poolDic[original.name].Pop(parent);
    }

    // name에 해당하는 원본 GameObject를 반환
    public GameObject GetOriginal(string name)
    {
        if (_poolDic.ContainsKey(name) == false)
            return null;
        
        return _poolDic[name].Original;
    }
    
    public void Clear()
    {
        foreach (Transform child in _root)
            Object.Destroy(child.gameObject);

        _poolDic.Clear();
    }
}
