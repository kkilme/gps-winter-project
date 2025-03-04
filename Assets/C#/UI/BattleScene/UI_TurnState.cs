using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TurnState : UI_Base
{
    private Dictionary<Creature, UI_CreatureTurnFrame> _turnFrames = new Dictionary<Creature, UI_CreatureTurnFrame>();
    private RectTransform _rectTransform;
    private RectTransform _bgFrame;
    private const float NORMAL_FRAME_SIZE = 90f;
    private const float BIG_FRAME_SIZE = 150f;
    private const float PADDING = 5f;
    private const float BORDER_SIZE = 40f;

    enum Objects
    {
        Frame
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(Objects));
        _bgFrame = GetGameObject(Objects.Frame).GetComponent<RectTransform>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Setup()
    {
        foreach (Creature creature in Managers.BattleMng.TurnSystem.Turns)
        {
            UI_CreatureTurnFrame creatureTurnFrame = Managers.UIMng.MakeSubItemUI<UI_CreatureTurnFrame>(_bgFrame, "Battle/" + nameof(UI_CreatureTurnFrame));
            creatureTurnFrame.Setup(creature);
            _turnFrames.Add(creature, creatureTurnFrame);
        }
        ResizeBGFrame();
    }

    // 턴 프레임들을 턴 순서에 맞게 배치
    public void MoveTurnFrames()
    {
        int index = 0;
        foreach (Creature creature in Managers.BattleMng.TurnSystem.Turns)
        {
            UI_CreatureTurnFrame creatureTurnFrame = _turnFrames[creature];
            bool isCurrentTurn = creature == Managers.BattleMng.CurrentTurnCreature;
            float x;

            // 현재 턴일 시 프레임 크기를 크게 하고 깜빡임 효과
            if (isCurrentTurn)
            {
                x = BORDER_SIZE;
                creatureTurnFrame.Resize(BIG_FRAME_SIZE);
                creatureTurnFrame.StartBlinking();
            } else
            {
                x = BORDER_SIZE + BIG_FRAME_SIZE / 2 + PADDING + NORMAL_FRAME_SIZE / 2 + (NORMAL_FRAME_SIZE + PADDING) * index;
                creatureTurnFrame.Resize(NORMAL_FRAME_SIZE);
                creatureTurnFrame.StopBlinking();
                index++;
            }

            creatureTurnFrame.MoveTo(x);
        }
    }

    // Creature 수에 맞게 배경 프레임 크기 조절
    private void ResizeBGFrame()
    {
        Vector2 sizeDelta = _rectTransform.sizeDelta;
        sizeDelta.x = BIG_FRAME_SIZE / 2 + (NORMAL_FRAME_SIZE + PADDING) * (_turnFrames.Count - 1) + BORDER_SIZE * 2;
        _rectTransform.DOSizeDelta(sizeDelta, 0.5f);
    }
    public override Tween Show()
    {
        gameObject.SetActive(true);
        return _rectTransform.DOAnchorPosY(500, 1f).From(true).SetEase(Ease.OutBack);
    }

    public override Tween Hide()
    {
        return _rectTransform.DOAnchorPosY(500, 0.7f).SetEase(Ease.InBack);
    }

}
