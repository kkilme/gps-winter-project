using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Flee : BaseSkill
{
    public override ActionEffectRange EffectRange { get; protected set; } = new DummyRange();
    public override ActionTargetSelector TargetSelector { get; protected set; } = new DummySelector();

    public override IEnumerator Execute()
    {
        Vector3 fleeTargetPos = Executor.transform.position - Executor.transform.forward * 50f;

        // 코인 던지기
        var coinResult = CoinTossHelper.CoinToss(SkillData.CoinCount, Executor.CreatureStat.NameToStat(SkillData.UsingStat));

        // 코인 던지기 UI 애니메이션 재생
        yield return Managers.BattleMng.UI.CoinTossDisplay.ShowResult(coinResult.result, SkillData.UsingStat);

        // 코인 토스가 전부 성공하지 않으면 도망 실패
        if (coinResult.successCount != SkillData.CoinCount)
        {
            yield return new WaitForSeconds(1f);
            Managers.BattleMng.OnActionEnd();
            yield break;
        }

        // 도망갈 방향 바라보기
        yield return Executor.transform.DOLookAt(fleeTargetPos, 0.5f).SetEase(Ease.OutQuad).WaitForCompletion();

        // 도망가기
        Executor.transform.DOMove(fleeTargetPos, GameUtility.CalculateMovetime(Executor.transform.position, fleeTargetPos));
        _animator.SetBool(GlobalValues.ANIMATION_PARAM_MOVING, true);
      
        yield return new WaitForSeconds(2f);

        _animator.SetBool(GlobalValues.ANIMATION_PARAM_MOVING, false);

        Managers.BattleMng.RemoveHero(Executor as Hero, true);
    }
}