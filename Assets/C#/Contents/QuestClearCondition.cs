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
    }

    [System.Serializable] // ���� ������ ������� ���� óġ
    public class KillMonsters : ClearCondition
    {
        [SerializeField] private int _killCount;
        private int _currentCount;

        public override bool isClear()
        {
            return _currentCount >= _killCount || _killCount == 0;
        }

        // �ش� �Լ��� �̺�Ʈ �ݹ��� �̿��ؼ� ȣ��
        private void AddCount()
        {
            _currentCount++;
        }

        public override string ToString()
        {
            if (_killCount == 0)
                return "";

            return $"{_currentCount} / {_killCount}";
        }
    }

    [System.Serializable] // Ư�� ���� óġ
    public class KillSpecificMonster : ClearCondition
    {
        [SerializedDictionary("MonsterName", "ConditionCount")]
        [SerializeField] private SerializedDictionary<Define.MonsterName, int[]> _killCount; // 0�� �ε���: ���� ī��Ʈ, 1�� �ε���: ���� ī��Ʈ

        public override bool isClear()
        {
            if (_killCount.Count == 0) 
                return true;

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
            if (_killCount.Count == 0)
                return "";

            string toString = "";

            foreach (var item in _killCount)
                toString += $"{item.Key}: {item.Value[0]}\n";

            return toString;
        }
    }

    [System.Serializable]
    public class hasSpecificItem : ClearCondition
    {
        [SerializedDictionary("ItemName", "ConditionCount")]
        [SerializeField] private SerializedDictionary<Define.ItemName, int[]> _havingCount; // 0�� �ε���: ���� ī��Ʈ, 1�� �ε���: ���� ī��Ʈ

        public override bool isClear()
        {
            if (_havingCount.Count == 0) 
                return true;

            foreach (var counts in _havingCount.Values)
                if (counts[0] > counts[1])
                    return false;

            return true;
        }

        public override string ToString()
        {
            if (_havingCount.Count == 0)
                return "";

            return "";
        }
    }
}

