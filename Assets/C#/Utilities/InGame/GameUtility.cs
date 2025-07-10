using System.Collections;
using UnityEngine;
using System;


public static class GameUtility
{
    /// <summary>
    /// t1의 위치에서 t2 위치까지의 Creature 이동 시간 계산 
    /// </summary>
    /// <remarks>
    /// Creature의 이동속도는 GlobalValues.CREATURE_BATTLE_VELOCITY로 정의됨.
    /// </remarks>
    public static float CalculateMovetime(Transform t1, Transform t2)
    {
        return CalculateMovetime(t1.position, t2.position);
    }

    /// <summary>
    /// v1, v2사이의 Creature 이동 시간 계산.
    /// </summary>
    /// <remarks>
    /// Creature의 이동속도는 GlobalValues.CREATURE_BATTLE_VELOCITY로 정의됨.
    /// </remarks>
    public static float CalculateMovetime(Vector3 v1, Vector3 v2)
    {
        return Vector3.Distance(v1, v2) / GlobalValues.CREATURE_BATTLE_VELOCITY;
    }
}
