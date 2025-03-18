using System.Collections;
using UnityEngine;


public static class GameUtility
{
    public static float CalculateMovetime(Transform t1, Transform t2)
    {
        return CalculateMovetime(t1.position, t2.position);
    }

    /// <summary>
    /// 두 지점 사이의 이동 시간을 계산. Creature의 이동속도는 GlobalValues.CREATURE_BATTLE_VELOCITY로 정의됨.
    /// </summary>
    public static float CalculateMovetime(Vector3 v1, Vector3 v2)
    {
        return Vector3.Distance(v1, v2) / GlobalValues.CREATURE_BATTLE_VELOCITY;
    }

}
