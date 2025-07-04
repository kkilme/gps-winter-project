using System.Collections;
using UnityEngine;


public static class MonsterSpawner
{
    public static Monster SpawnMonster(int monsterDataId)
    {
        if (!Managers.DataMng.MonsterDataDict.ContainsKey(monsterDataId))
        {
            Debug.LogError($"Monster data doesn't exist. MonsterDataId: {monsterDataId}");
            return null;
        }

        string monsterName = Managers.DataMng.MonsterDataDict[monsterDataId].Name;
        GameObject go = Managers.ResourceMng.Instantiate(GlobalValues.MONSTER_PREFAB_PATH_PREFIX + monsterName);
        Monster monster = go.GetComponent<Monster>();

        monster.SetData(monsterDataId);
        go.transform.position = Vector3.zero;
        monster.transform.parent = GlobalUtility.FindOrCreateTransform("@Monsters");

        return monster;
    }
}
