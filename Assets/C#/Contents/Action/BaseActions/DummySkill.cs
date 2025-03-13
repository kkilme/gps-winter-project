using DG.Tweening;
using System.Collections;
using UnityEngine;

/// <summary>
/// 아무것도 하지 않는 더미 스킬
/// </summary>
public class DummySkill : BaseSkill
{
    public override ActionEffectRange EffectRange { get; protected set; } = new DummyRange();
    public override ActionTargetSelector TargetSelector { get; protected set; } = new DummySelector();

    public override IEnumerator Execute()
    {
        yield return new WaitForSeconds(1f);
    }
}