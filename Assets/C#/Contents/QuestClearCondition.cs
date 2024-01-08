using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace QuestClearCondition
{
    [System.Serializable]
    public abstract class ClearCondition
    {
        public abstract bool isClear();
        public abstract override string ToString(); // ���� ����� ����Ʈ ������ ���
        public abstract bool isNull();
    }

    [System.Serializable] // ���� ������ ������� ���� óġ
    public class KillMonsters : ClearCondition
    {
        [SerializeField] private int _killCount;
        private int _currentCount;

        public override bool isClear()
        {
            return _currentCount >= _killCount;
        }

        // �ش� �Լ��� �̺�Ʈ �ݹ��� �̿��ؼ� ȣ��
        private void AddCount()
        {
            _currentCount++;
        }

        public override string ToString()
        {
            return $"{_currentCount} / {_killCount}";
        }

        public override bool isNull() => _killCount == 0;
    }

    [System.Serializable] // Ư�� ���� óġ
    public class KillSpecificMonster : ClearCondition
    {
        [SerializedDictionary("MonsterName", "ConditionCount")]
        [SerializeField] private SerializedDictionary<Define.MonsterName, int[]> _killCount; // 0�� �ε���: ���� ī��Ʈ, 1�� �ε���: ���� ī��Ʈ

        public override bool isClear()
        {
            foreach (var counts in _killCount.Values)
                if (counts[0] > counts[1])
                    return false;

            return true;
        }

        public void AddCount(Define.MonsterName monsterName)
        {
            if (_killCount.TryGetValue(monsterName, out var counts))
            {
                counts[1]++;
            }
        }

        public override string ToString()
        {
            string toString = "";

            foreach (var item in _killCount)
                toString += $"{item.Key}: {item.Value[0]}\n";

            return toString;
        }

        public override bool isNull() => _killCount.Count == 0;
    }

    [System.Serializable]
    public class hasSpecificItem : ClearCondition
    {
        [SerializedDictionary("ItemName", "ConditionCount")]
        [SerializeField] private SerializedDictionary<Define.ItemName, int[]> _havingCount; // 0�� �ε���: ���� ī��Ʈ, 1�� �ε���: ���� ī��Ʈ

        public override bool isClear()
        {
            foreach (var counts in _havingCount.Values)
                if (counts[0] > counts[1])
                    return false;

            return true;
        }

        public override string ToString()
        {
            return "";
        }

        public override bool isNull() => _havingCount.Count == 0;
    }
}

