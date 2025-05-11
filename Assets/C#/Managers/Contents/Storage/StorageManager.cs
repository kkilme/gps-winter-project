using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어가 소유한 모든 아이템의 데이터를 저장 및 관리
public class StorageManager
{
    public HeroStorage HeroStorage { get; protected set; } = new HeroStorage();

    private int _gold;
    public int Gold
    {
        get => _gold; 
        set
        {
            if(value < 0)
            {
                Debug.LogWarning("Trying to make gold negative!");
                _gold = 0;
            }
            else _gold = value;
        }
    }

    // TODO: 아이템 관리 관련은 아직 미구현
}
