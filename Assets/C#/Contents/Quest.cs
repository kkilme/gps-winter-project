using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuestClearCondition;

/*
����Ʈ �̸�
����Ʈ ����
����Ʈ ǥ�� ����
- Ư�� ���� ����
- Ư�� ����Ʈ Ŭ����
- Ư�� ���� ����
����Ʈ Ŭ���� ����
- ���� n���� ���
- Ư�� ���� n���� ���
- Ư�� ������ ����
- Ư�� ���� ����
����Ʈ ����
- ��
- ����ġ
- ������
- ��ų
 */
[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Object/Quest")]
public class Quest : ScriptableObject
{
    [field: Header("Information")]
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField, TextArea(3, 10)] public string Description { get; private set; }

    [field: Header("Condition")]
    [SerializeField] private KillSpecificMonster _killSpecificMonster;
    [SerializeField] private KillMonsters _killMonsters;
    [SerializeField] private hasSpecificItem _hasSpecificItem;
    public ClearCondition[] ClearCondition { get; private set; }

    [field: Header("Reward")]
    [field: SerializeField] public SerializedDictionary<Define.QuestReward, int> Reward { get; private set; }

    private void Awake()
    {
        ClearCondition = new ClearCondition[]
        {
            _killSpecificMonster,
            _killMonsters,
            _hasSpecificItem
        };
    }

    public bool isClear()
    {
        foreach (var condition in ClearCondition)
            if (!condition.isClear())
                return false;

        return true;
    }
}

namespace QuestExtention
{
    public static class RewardExtension
    {
        public static string RewardToString(this SerializedDictionary<Define.QuestReward, int> reward)
        {
            string toString = "";

            foreach (var item in reward)
            {
                toString += $"{item.Key} : {item.Value} ";
            }

            return toString;
        }
    }
}