using System.Collections;
using UnityEngine;

// 프로토타입 버전에선 몬스터의 AI만 구현하지만, Hero의 AI도 구현하여 자동 전투 등을 구현할 수 있을 것으로 생각됨.
public abstract class CreatureAI : MonoBehaviour
{
    protected Creature _owner;
    protected BattleManager _battleManager => Managers.BattleMng;
    protected BattleGridSystem _gridSystem => _battleManager.GridSystem;

    private void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        _owner = GetComponent<Creature>();
    }

    /// <summary>
    /// 실행할 스킬 결정
    /// </summary>
    /// <returns>선택된 스킬</returns>
    public abstract BaseSkill DecideSkill(); // 자동 전투를 구현한다고 해도 Item은 AI가 사용할 수 없게 할 생각이기에 BaseSkill 반환
}
